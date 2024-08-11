using Microsoft.IdentityModel.Tokens;
using Services.API.Security.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.API.Security.Core.JwtLogic
{
    public class JwtGenerator : IJwtGenerator
    {
        public string GenerateJwt(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("username", user.UserName),
                new Claim("roleAdmin" ,user.RoleAdmin.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lSQOzerZ0NiZ5Ot8SQhlM1rCAUjRnl6cKibUHcgVrn+0DL8y0bUaFZeOOlHQ5zBeSn6tKObZFYfX52iLtuNcCQ=="));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}
