namespace PhotoGallery.Entities
{
    public class JWTConfiguration
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
        public double? Expires { get; set; }
    }
}
