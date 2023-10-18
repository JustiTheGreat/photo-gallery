using Firebase.Storage;
using System.Net;

namespace PhotoGallery.Services
{
    public class ImageService : IImageService
    {
        private readonly FirebaseStorage _storage;

        public ImageService(FirebaseStorage storage)
        {
            _storage = storage;
        }

        public async Task<string> PutImage(string imageName, Stream imageStream)
        {
            string downloadUrl = await _storage.Child(imageName).PutAsync(imageStream);
            return DownloadImage(downloadUrl);
        }

        public async Task<string> GetImageString(string imageName)
        {
            string downloadUrl = await _storage.Child(imageName).GetDownloadUrlAsync();
            return DownloadImage(downloadUrl);
        }

        public void DeleteImage(string imageReference)
        {
            _storage.Child(imageReference).DeleteAsync();
        }

        private static string DownloadImage(string downloadUrl)
        {
            byte[] imageBytes = new WebClient().DownloadData(downloadUrl);
            string image = Convert.ToBase64String(imageBytes);
            return image;
        }
    }
}
