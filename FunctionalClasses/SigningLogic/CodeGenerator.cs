using FindProgrammingProject.Models.ObjectModels;
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
            var Token = await userManager.GeneratePasswordResetTokenAsync(user);

            SigningResult result = await sender.SendCode(user.Email, Token, true);
            return result;

        }
        public async Task<string> GetCode(User user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
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
                var Token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                SigningResult result = await sender.SendCode(user.Email, Token, false);
                return result;

            }
            catch(Exception e)
            {
                return SigningResult.Error;
            }
        }

        public async Task<string> GetCode(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }
    }
    public interface ICodeGenerator
    {
        public Task<SigningResult> GenerateCode(User user);
        public Task<string> GetCode(User user);
    }


}
