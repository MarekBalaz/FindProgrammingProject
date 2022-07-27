using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class PasswordResetCodeGenerator : ICodeGenerator
    {
        private UserManager<User> userManager;
        private ISender sender;
        public PasswordResetCodeGenerator(UserManager<User> userManager, ISender sender)
        {
            this.userManager = userManager;
            this.sender = sender;
        }
        public async Task<SigningResult> GenerateCode(User user)
        {
            var encodedToken = HttpUtility.UrlEncode(await userManager.GeneratePasswordResetTokenAsync(user));
            var encodedEmail = HttpUtility.UrlEncode(user.Email);

            SigningResult result = await sender.SendCode(encodedEmail,encodedToken);
            return result;

        }
    }
    public class EmailVerificationCodeGenerator : ICodeGenerator
    {
        private UserManager<User> userManager;
        private ISender sender;

        public EmailVerificationCodeGenerator(UserManager<User> userManager, ISender sender)
        {
            this.userManager = userManager;
            this.sender = sender;
        }
        public async Task<SigningResult> GenerateCode(User user)
        {
            try
            {
                var EncodedToken = HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(user));
                var EncodedEmail = HttpUtility.UrlEncode(user.Email);

                SigningResult result = await sender.SendCode(EncodedEmail, EncodedToken);
                return result;

            }
            catch(Exception e)
            {
                return SigningResult.Error;
            }
        }
    }
    public interface ICodeGenerator
    {
        public Task<SigningResult> GenerateCode(User user);
    }


}
