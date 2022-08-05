using FindProgrammingProject.Models;
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

                new Claim(ClaimTypes.Role, "user")

            };



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a8d1fe1a-9523-4a2a-a72c-3b0fedd75bd5"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(issuer: "https://localhost:7168", audience: "https://localhost:7168", claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
    public interface IJwtTokenGenerator
    {
        string GetJwtToken(User user);
    }
}
