using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    
    public class ResetPassword : IReset
    {
        private PasswordResetTokenVerifiction verification;
        private UserManager<User> userManager;
        public ResetPassword(UserManager<User> userManager, PasswordResetTokenVerifiction verification)
        {
            this.userManager = userManager;
            this.verification = verification;

        }
        public async Task<SigningResult> Reset(string newPassword, string newPasswordRepeated, string token, string email)
        {
            if(newPassword == newPasswordRepeated)
            {
                var result = await verification.Verify(HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(token));
                if(result == SigningResult.Success)
                {
                    User user = await userManager.FindByEmailAsync(email);
                    IdentityResult resetResult = await userManager.ResetPasswordAsync(user,token,newPassword);
                    if(resetResult.Succeeded == true)
                    {
                        return SigningResult.Success;
                    }
                }
                return SigningResult.IncorrectToken;
            }
            return SigningResult.PasswordsDoNotMatch;
        }
    }
    public interface IReset
    {
        public Task<SigningResult> Reset(string newPassword, string newPasswordRepeated, string token, string email);
    }
    
}
