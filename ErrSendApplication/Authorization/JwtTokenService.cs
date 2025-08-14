using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using ErrSendApplication.Common.Configs;

namespace ErrSendApplication.Authorization
{
    public class JwtTokenService : Interfaces.Authorization.IJwtTokenService
    {
        private readonly string secret;
        private readonly int expiryMinutes;
        private static readonly HashSet<string> UsedLoginPasswordPairs = new();
        private readonly IValidator<JwtConfig> configValidator;

        public JwtTokenService(string secret, int expiryMinutes, IValidator<JwtConfig> configValidator)
        {
            this.secret = secret;
            this.expiryMinutes = expiryMinutes;
            this.configValidator = configValidator;
        }

        public string GenerateToken(string login, string password, IEnumerable<string> roles)
        {
            // Валідація конфігурації через FluentValidation замість ручних перевірок
            var config = new JwtConfig { Secret = secret, ExpiryMinutes = expiryMinutes };
            var validationResult = configValidator.Validate(config);
            
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new InvalidOperationException($"JWT конфігурація невалідна: {errors}");
            }

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