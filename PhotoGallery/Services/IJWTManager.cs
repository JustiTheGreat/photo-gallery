namespace PhotoGallery.Services
{
    public interface IJWTManager
    {
        string GenerateJWT(string uid);
        string GetClaimFromJWT(string jwt, string claimName);
    }
}
