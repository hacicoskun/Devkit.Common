using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Devkit.Common.Identity.Core.Entities;
using Devkit.Common.Identity.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Devkit.Common.Identity.Providers.AspNetIdentity.Utilities
{
    public class JwtTokenGenerator
    {
        private readonly AspNetIdentityOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<IdentityOptions> options)
        {
            _jwtOptions = options.Value.AspNetIdentity;
        }

        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.JwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles) claims.Add(new Claim("role", role));

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
            var token = new JwtSecurityToken(_jwtOptions.JwtIssuer, _jwtOptions.JwtAudience, claims, expires: expires, signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), (int)(expires - DateTime.UtcNow).TotalSeconds);
        }
    }
}