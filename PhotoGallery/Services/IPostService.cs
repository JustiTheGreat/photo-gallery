using PhotoGallery.DTOs;

namespace PhotoGallery.Services
{
    public interface IPostService
    {
        Task<PostResponseDTO> CreatePost(PostRequestDTO postDTO, string uid);
        Task<PostResponseDTO> GetPostById(string postId);
        Task<List<PostResponseDTO>> GetAllPosts(string filter);
    }
}
