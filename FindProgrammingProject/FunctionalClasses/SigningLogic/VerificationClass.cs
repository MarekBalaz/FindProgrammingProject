using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class EmailTokenVerification : IVerification
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private SignClass signClass;

        public EmailTokenVerification(UserManager<User> userManager, SignInManager<User> signInManager, SignClass signClass)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.signClass = signClass;
        }

        public async Task<VerificationResult> Verify(string Email, string Token)
        {
            var decodedEmail = HttpUtility.UrlDecode(Email);
            var decodedToken = HttpUtility.UrlDecode(Token);


            var user = await userManager.FindByEmailAsync(decodedEmail);
            bool response = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, UserManager<User>.ConfirmEmailTokenPurpose, decodedToken);
            if(response == true)
            {
                await userManager.ConfirmEmailAsync(user,Token);
                await userManager.UpdateAsync(user);
                await signInManager.SignInAsync(user,true);
                return VerificationResult.Success;
            }
            else
            {
                await userManager.DeleteAsync(user);
                return VerificationResult.Failure;
            }

        }
    }
    public class PasswordResetTokenVerifiction : IVerification
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private SignClass signClass;

        public PasswordResetTokenVerifiction(UserManager<User> userManager, SignInManager<User> signInManager, SignClass signClass)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.signClass = signClass;
        }
        public async Task<VerificationResult> Verify(string Email, string Token)
        {
            var decodedEmail = HttpUtility.UrlDecode(Email);
            var decodedToken = HttpUtility.UrlDecode(Token);

            var user = await userManager.FindByEmailAsync(decodedEmail);
            
            bool response = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, UserManager<User>.ResetPasswordTokenPurpose, decodedToken);
            
            if(response == true)
            {
                return VerificationResult.Success;
            }
            return VerificationResult.Failure;

        }
    }

    public interface IVerification
    {
        public Task<VerificationResult> Verify(string Email, string Token);
    }
    public enum VerificationResult
    {
        Success,
        Failure
    }
    
}
