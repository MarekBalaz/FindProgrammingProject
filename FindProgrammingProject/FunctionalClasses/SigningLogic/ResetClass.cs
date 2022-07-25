using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public enum ResetResponse
    {
        PasswordsDoNotMatch,
        Success,
        IncorrectToken
    }
    public class ResetPassword : IReset
    {
        private PasswordResetTokenVerifiction verification;
        private UserManager<User> userManager;
        public ResetPassword(UserManager<User> userManager, PasswordResetTokenVerifiction verification)
        {
            this.userManager = userManager;
            this.verification = verification;

        }
        public async Task<ResetResponse> Reset(string newPassword, string newPasswordRepeated, string token, string email)
        {
            if(newPassword == newPasswordRepeated)
            {
                var result = await verification.Verify(HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(token));
                if(result == VerificationResult.Success)
                {
                    User user = await userManager.FindByEmailAsync(email);
                    var resetResult = await userManager.ResetPasswordAsync(user,token,newPassword);
                    if(resetResult.Succeeded)
                    {
                        return ResetResponse.Success;
                    }
                }
                return ResetResponse.IncorrectToken;
            }
            return ResetResponse.PasswordsDoNotMatch;
        }
    }
    public interface IReset
    {
        public Task<ResetResponse> Reset(string newPassword, string newPasswordRepeated, string token, string email);
    }
    
}
