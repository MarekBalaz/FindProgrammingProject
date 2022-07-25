using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class PaswordResetCodeGenerator : ICodeGenerator
    {
        private UserManager<User> userManager;
        private ISender sender;
        public PaswordResetCodeGenerator(UserManager<User> userManager, ISender sender)
        {
            this.userManager = userManager;
            this.sender = sender;
        }
        public async Task<SignUpResult> GenerateCode(User user)
        {
            var encodedToken = HttpUtility.UrlEncode(await userManager.GeneratePasswordResetTokenAsync(user));
            var encodedEmail = HttpUtility.UrlEncode(user.Email);

            SignUpResult result = await sender.SendCode(encodedEmail,encodedToken);
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
        public async Task<SignUpResult> GenerateCode(User user)
        {
<<<<<<< HEAD
            var EncodeToken = HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(user));
            var encodedEmail = HttpUtility.UrlEncode(user.Email);

            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            MailMessage mailMessage = new MailMessage(new MailAddress("marekgamingacc@gmail.com"), new MailAddress(HttpUtility.HtmlDecode(encodedEmail)));
            mailMessage.Subject = "Email Verification";
            //here we will add code into html code
            mailMessage.Body = "";
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(appSettings["ApplicationInfo:EmailInfo:Email"], appSettings["ApplicationInfo:EmailInfo:EmailPassword"]);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = nc;
=======
>>>>>>> 43d50c8f43db04dfc2f96b8a254aaee84f8a1290
            try
            {
                var EncodedToken = HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(user));
                var EncodedEmail = HttpUtility.UrlEncode(user.Email);

                SignUpResult result = await sender.SendCode(EncodedEmail, EncodedToken);
                return result;

            }
            catch(Exception e)
            {
                return SignUpResult.Error;
            }
        }
    }
    public interface ICodeGenerator
    {
        public Task<SignUpResult> GenerateCode(User user);
    }


}
