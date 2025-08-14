using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ErrSendApplication.Authorization
{
    public class JwtTokenService : Interfaces.Authorization.IJwtTokenService
    {
        private readonly string secret;
        private readonly int expiryMinutes;
        private static readonly HashSet<string> UsedLoginPasswordPairs = new();

        public JwtTokenService(string secret, int expiryMinutes)
        {
            this.secret = secret;
            this.expiryMinutes = expiryMinutes;
        }

        public string GenerateToken(string login, string password, IEnumerable<string> roles)
        {
            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 16)
                throw new InvalidOperationException("Секрет JWT відсутній або занадто короткий. Встановіть довгий секрет у змінній середовища JWT_SECRET або appsettings.json!");

            var claims = new List<Claim>
            {
                new Claim("login", login),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddMinutes(expiryMinutes)).ToUnixTimeSeconds().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 