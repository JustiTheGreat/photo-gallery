namespace PhotoGallery.Services
{
    public interface IImageService
    {
        Task<string> PutImage(string imageName, Stream imageStream);
        Task<string> GetImageString(string imageReference);
        void DeleteImage(string imageReference);
    }
}
