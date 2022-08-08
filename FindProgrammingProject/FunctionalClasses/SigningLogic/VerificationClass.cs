using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class EmailTokenVerification : IVerification
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public EmailTokenVerification(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<SigningResult> Verify(string Email, string Token)
        {
            var decodedEmail = HttpUtility.UrlDecode(Email);
            var decodedToken = HttpUtility.UrlDecode(Token);


            var user = await userManager.FindByEmailAsync(decodedEmail);
            bool response = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, UserManager<User>.ConfirmEmailTokenPurpose, decodedToken);
            if(response == true)
            {
                await userManager.ConfirmEmailAsync(user,Token);
                await userManager.UpdateAsync(user);
                return SigningResult.Success;
            }
            else
            {
                return SigningResult.IncorrectToken;
            }

        }
    }
    public class PasswordResetTokenVerifiction : IVerification
    {
        private UserManager<User> userManager;

        public PasswordResetTokenVerifiction(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public virtual async Task<SigningResult> Verify(string Email, string Token)
        {
            var decodedEmail = HttpUtility.UrlDecode(Email);
            var decodedToken = HttpUtility.UrlDecode(Token);

            var user = await userManager.FindByEmailAsync(decodedEmail);
            
            bool response = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, UserManager<User>.ResetPasswordTokenPurpose, decodedToken);
            
            if(response == true)
            {
                return SigningResult.Success;
            }
            return SigningResult.IncorrectToken;

        }
    }
    public class AuthorizationTokenVerification : IVerification
    {
        public async Task<SigningResult> Verify(string Email, string Token)
        {
            SecurityToken token;
            
            var response = new JwtSecurityTokenHandler().ValidateToken(Token,new TokenValidationParameters { ValidateLifetime = true}, out token);
            if(response == null)
            {
                return SigningResult.IncorrectToken;
            }
            return SigningResult.Success;
        }
    }

    public interface IVerification
    {
        public Task<SigningResult> Verify(string Email, string Token);
    }
    
}
