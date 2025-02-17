using BackTareas.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackTareas.Utilities
{
    public class CreatedToken
    {

        private readonly IConfiguration _configuration;
        public CreatedToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string EncryptSHA256(string texto)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenereatedJWT(User model)
        {
            //Se crea info de usuario para el token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,model.Id.ToString()),
                new Claim(ClaimTypes.Email, model.Email!)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Crear detalles del Token
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
