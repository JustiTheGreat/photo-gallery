using Microsoft.IdentityModel.Tokens;
using PhotoGallery.Common;
using PhotoGallery.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhotoGallery.Services
{
    public class JWTManager : IJWTManager
    {
        private readonly JWTConfiguration _jwtConfiguration;
        public JWTManager(IConfiguration configuration)
        {
            _jwtConfiguration = configuration.GetSection(PGConstants.JWT).Get<JWTConfiguration>()
                ?? throw new NullReferenceException($"{nameof(JWTManager)}:{nameof(_jwtConfiguration)}");

        }

        public string GenerateJWT(string uid)
        {
            string issuer = _jwtConfiguration.Issuer
                ?? throw new NullReferenceException($"{nameof(JWTManager)}:{nameof(GenerateJWT)}:{nameof(issuer)}");
            string audience = _jwtConfiguration.Audience
                ?? throw new NullReferenceException($"{nameof(JWTManager)}:{nameof(GenerateJWT)}:{nameof(audience)}");
            string secretKey = _jwtConfiguration.SecretKey
                ?? throw new NullReferenceException($"{nameof(JWTManager)}:{nameof(GenerateJWT)}:{nameof(secretKey)}");
            double expires = _jwtConfiguration.Expires
                ?? throw new NullReferenceException($"{nameof(JWTManager)}:{nameof(GenerateJWT)}:{nameof(expires)}");

            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.SecretKey);
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(expires);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(PGConstants.UIdClaimName, uid),
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);
            return token;
        }

        public string GetClaimFromJWT(string jwt, string claimName)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwt);
            var claimValue = jwtSecurityToken.Claims.First(claim => claim.Type.Equals(claimName)).Value;
            return claimValue;
        }
    }
}
