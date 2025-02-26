using Auth.Settings;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Service
{
    public class JwtService
    {
        private readonly IOptions<AuthSettings> _options;

        public JwtService(IOptions<AuthSettings> options)
        {
            _options = options;
        }
        public string GenerateToken(Account account)
        {
            var claims = new List<Claim>
            {
                new Claim("userName", account.UserName),
                new Claim("firstName", account.FirstName),
                new Claim("id", account.Id.ToString()),
                new Claim("isAdmin", (account.IsAdmin == null || account.IsAdmin == false) ? "false" : "true"),
            };

            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(_options.Value.Expires),
                claims: claims,
                signingCredentials:
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
                            SecurityAlgorithms.HmacSha256)
                    );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var userName = principal.FindFirst("userName")?.Value;
                    var userId = principal.FindFirst("id")?.Value;
                    var isAdmin = principal.FindFirst("isAdmin")?.Value;
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
