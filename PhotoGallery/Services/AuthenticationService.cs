using Google.Cloud.Firestore;
using PhotoGallery.Common;
using PhotoGallery.Models;

namespace PhotoGallery.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly FirestoreDb _db;

        public AuthenticationService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<string> Register(RegisterDTO registerDTO)
        {
            CollectionReference collection = _db.Collection(PGConstants.UserCollection);
            Query query = collection.WhereEqualTo(PGConstants.UserCollectionEmailField, registerDTO.Email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            if (querySnapshot.Count != 0) throw new PGException("email already registered");
            DocumentReference document = await collection.AddAsync(registerDTO);
            return document.Id;
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            CollectionReference collection = _db.Collection(PGConstants.UserCollection);
            Query query = collection
                .WhereEqualTo(PGConstants.UserCollectionEmailField, loginDTO.Email)
                .WhereEqualTo(PGConstants.UserCollectionPasswordField, loginDTO.Password);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            if (querySnapshot.Count == 0) throw new PGException("invalid credentials");
            if (querySnapshot.Count > 1) throw new PGException("database inconsistency: too many matches during login");
            DocumentReference document = querySnapshot.Documents.First().Reference;
            return document.Id;
        }
    }
}
