using PhotoGallery.Models;

namespace PhotoGallery.Services
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
    }
}
