using FindProgrammingProject.Models.ObjectModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        public string GetJwtToken(User user)
        {
            var claims = new[] 
            {

                new Claim(ClaimTypes.Name, user.UserName),

                new Claim(ClaimTypes.Role, "user"),

                new Claim("UniqueIdentifier",user.Id),

                new Claim("aud", "https://localhost:7137"),

                new Claim("aud", "https://localhost:56667")

            };



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a8d1fe1a-9523-4a2a-a72c-3b0fedd75bd5"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(issuer: "https://localhost:7137", audience: null, claims: claims, expires: DateTime.Now.AddMinutes(10), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string RefreshTokenGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
    public interface IJwtTokenGenerator
    {
        string GetJwtToken(User user);
        string RefreshTokenGenerator();
    }
}
