using Google.Cloud.Firestore;

namespace PhotoGallery.Models
{
    [FirestoreData]
    public class RegisterDTO
    {
        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? Password { get; set; }
    }
}
