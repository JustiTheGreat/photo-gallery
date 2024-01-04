using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using PhotoGallery.Common;
using PhotoGallery.DTOs;

namespace PhotoGallery.Services
{
    public class PostService : IPostService
    {
        private readonly FirestoreDb _db;
        private readonly IImageService _imageService;

        public PostService(FirestoreDb db, IImageService imageService)
        {
            _db = db;
            _imageService = imageService;
        }

        public async Task<PostResponseDTO> CreatePost(PostRequestDTO postRequestDTO, string userId)
        {
            if (postRequestDTO.Description is null)
                throw new NullReferenceException($"{nameof(PostService)}: {nameof(CreatePost)}: {nameof(postRequestDTO.Description)}");
            if (postRequestDTO.Image is null)
                throw new NullReferenceException($"{nameof(PostService)}: {nameof(CreatePost)}: {nameof(postRequestDTO.Image)}");
            if (postRequestDTO.ImageExtension is null)
                throw new NullReferenceException($"{nameof(PostService)}: {nameof(CreatePost)}: {nameof(postRequestDTO.ImageExtension)}");

            CollectionReference collection = _db.Collection(PGConstants.PostCollection);
            DocumentReference document = await collection.AddAsync(
                new PostFirestore(postRequestDTO.Description, string.Empty, userId));
            string imageName = $"{document.Id}.{postRequestDTO.ImageExtension}";
            string imageString = await _imageService.PutImage(imageName, new MemoryStream(postRequestDTO.Image));
            document.UpdateAsync(PGConstants.PostCollectionStoragePathField, imageName).Wait();

            return new PostResponseDTO(document.Id, imageString, postRequestDTO.Description, imageName, userId);
        }

        public async Task<PostResponseDTO> GetPostById(string postId)
        {
            CollectionReference collection = _db.Collection(PGConstants.PostCollection);
            DocumentReference document = collection.Document(postId);
            DocumentSnapshot documentSnapshot = await document.GetSnapshotAsync();

            string storagePath = documentSnapshot.GetValue<string>(PGConstants.PostCollectionStoragePathField);
            return new PostResponseDTO(
                    documentSnapshot.Id,
                    _imageService.GetImageString(storagePath).Result,
                    documentSnapshot.GetValue<string>(PGConstants.PostCollectionDescriptionField),
                    storagePath,
                    documentSnapshot.GetValue<string>(PGConstants.PostCollectionUIdField)
                    );
        }

        public async Task<List<PostResponseDTO>> GetAllPosts()
        {
            List<PostResponseDTO> postResponseDTOs = new();
            CollectionReference collection = _db.Collection(PGConstants.PostCollection);
            QuerySnapshot querySnapshot = await collection.GetSnapshotAsync();
            List<DocumentSnapshot> posts = querySnapshot.Documents.ToList();
            foreach (DocumentSnapshot documentSnapshot in posts)
            {
                string storagePath = documentSnapshot.GetValue<string>(PGConstants.PostCollectionStoragePathField);
                postResponseDTOs.Add(new PostResponseDTO(
                        documentSnapshot.Id,
                        _imageService.GetImageString(storagePath).Result,
                        documentSnapshot.GetValue<string>(PGConstants.PostCollectionDescriptionField),
                        storagePath,
                        documentSnapshot.GetValue<string>(PGConstants.PostCollectionUIdField)
                        ));
            }
            return postResponseDTOs;
        }

        public async Task<List<PostResponseDTO>> GetFilteredPosts(string filter)
        {
            List<PostResponseDTO> postResponseDTOs = new();
            CollectionReference collection = _db.Collection(PGConstants.PostCollection);
            QuerySnapshot querySnapshot = await collection.GetSnapshotAsync();
            List<DocumentSnapshot> filteredAndSortedPosts = querySnapshot.Documents
                .Where(document => document.GetValue<string>("Description").Contains(filter))
                .Select(document => new
                {
                    Document = document,
                    Position = document.GetValue<string>("Description").IndexOf(filter)
                })
                .OrderBy(result => result.Position)
                .Select(result => result.Document)
                .ToList();
            foreach (DocumentSnapshot documentSnapshot in filteredAndSortedPosts)
            {
                string storagePath = documentSnapshot.GetValue<string>(PGConstants.PostCollectionStoragePathField);
                postResponseDTOs.Add(new PostResponseDTO(
                        documentSnapshot.Id,
                        _imageService.GetImageString(storagePath).Result,
                        documentSnapshot.GetValue<string>(PGConstants.PostCollectionDescriptionField),
                        storagePath,
                        documentSnapshot.GetValue<string>(PGConstants.PostCollectionUIdField)
                        ));
            }
            return postResponseDTOs;
        }
    }
}
