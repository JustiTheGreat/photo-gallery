namespace PhotoGallery.DTOs
{
    public class PostRequestDTO
    {
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
    }
}
