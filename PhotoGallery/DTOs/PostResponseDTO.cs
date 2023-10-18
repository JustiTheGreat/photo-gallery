namespace PhotoGallery.DTOs
{
    public class PostResponseDTO
    {
        public string? Id { get; set; }
        public string? ImageString { get; set; }
        public string? Description { get; set; }
        public string? StoragePath { get; set; }
        public string? UId { get; set; }
        public PostResponseDTO(string id, string imageString, string description, string storagePath, string uId)
        {
            Id = id;
            ImageString = imageString;
            Description = description;
            StoragePath = storagePath;
            UId = uId;
        }
    }
}
