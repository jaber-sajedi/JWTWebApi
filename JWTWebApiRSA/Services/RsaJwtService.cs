using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JWTWebApiRSA.Services
{
    public class RsaJwtService
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;

        public RsaJwtService()
        {
            _privateKey = RSA.Create();
            _publicKey = RSA.Create();

            _privateKey.ImportFromPem(File.ReadAllText("Keys/private.key"));
            _publicKey.ImportFromPem(File.ReadAllText("Keys/public.key"));
        }

        public string GenerateToken(string username)
        {
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username),
            }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(_privateKey),
                    SecurityAlgorithms.RsaSha256
                )
            });

            return handler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(_publicKey),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = handler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
