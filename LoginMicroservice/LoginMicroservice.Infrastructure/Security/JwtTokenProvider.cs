using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoginMicroservice.Infrastructure.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly IConfiguration _configuration;

        public JwtTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(AuthenticatedUser user, out DateTime expiresAt)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
            var issuer = _configuration["Jwt:Issuer"] ?? "GymClaude.Login";
            var audience = _configuration["Jwt:Audience"] ?? "GymClaude.WebUI";
            int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var expiresMinutes);
            if (expiresMinutes <= 0)
            {
                expiresMinutes = 60;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
