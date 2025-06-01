using JWTWebApiRSA.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWTWebApiRSA.Services
{
    public class JwtTokenService
    {
        private readonly RsaSecurityKey _privateKey;
        private readonly JwtSettings _settings;

        public JwtTokenService(RsaSecurityKey privateKey, JwtSettings settings)
        {
            _privateKey = privateKey;
            _settings = settings;
        }

        public string GenerateToken(string username)
        {
            var handler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("role", "admin")
        };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}