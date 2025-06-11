using Microsoft.IdentityModel.Tokens;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public class TokenService
    {
        private const string SecretKey = "MySecureSecretKeyForJWT256BitsThatIsStrong"; 
        private const string Issuer = "http://localhost:5083"; 
        private const string Audience = "http://127.0.0.1:5500/"; 
        static public string GenerateJwtToken(ResponseUserDataDTO user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()), // User ID
        new Claim(ClaimTypes.Name, user.UserName), // User name
        new Claim(ClaimTypes.Role, user.UserRole)  // User role(s)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool ValidateToken(string token)
        {
            try
            {
                // Create a TokenValidationParameters object to set validation criteria
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey); // Convert secret key to byte array

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Validate the token and get the claims principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // If validation is successful, return true
                return validatedToken != null;
            }
            catch (Exception ex)
            {
                // If there's any exception (e.g., token expired, invalid signature), return false
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }

    }
}
