using Google.Cloud.Firestore;

namespace PhotoGallery.DTOs
{
    [FirestoreData]
    public class PostFirestore
    {
        [FirestoreProperty]
        public string? Description { get; set; }
        [FirestoreProperty]
        public string? StoragePath { get; set; }
        [FirestoreProperty]
        public string? UId { get; set; }
        public PostFirestore() { }
        public PostFirestore(string? description, string? storagePath, string? uId)
        {
            Description = description;
            StoragePath = storagePath;
            UId = uId;
        }
    }
}
