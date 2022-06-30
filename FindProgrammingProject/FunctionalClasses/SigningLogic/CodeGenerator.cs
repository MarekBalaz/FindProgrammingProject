using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses
{
    public class PaswordResetCodeGenerator : ICodeGenerator
    {
        public Task<SignUpResult> GenerateCode(User user)
        {
            throw new NotImplementedException();
        }
    }
    public class EmailVerificationCodeGenerator : ICodeGenerator
    {
        private UserManager<User> userManager;

        public EmailVerificationCodeGenerator(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<SignUpResult> GenerateCode(User user)
        {
            var EncodeToken = HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(user));
            var encodedEmail = HttpUtility.UrlEncode(user.Email);

            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            MailMessage mailMessage = new MailMessage(appSettings["ApplicationInfo:EmailInfo:Email"], encodedEmail);
            mailMessage.Subject = "Email Verification";
            //here we will add code into html code
            mailMessage.Body = "";
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(appSettings["ApplicationInfo:EmailInfo:Email"],appSettings["ApplicationInfo:EmailInfo:EmailPassword"]);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = nc;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                return SignUpResult.EmailIncorrect;
            }
            return SignUpResult.Success;
        }
    }
    public interface ICodeGenerator
    {
        public Task<SignUpResult> GenerateCode(User user);
    }
    

}
