using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{

    public interface ISender
    {
        public Task<SigningResult> SendCode(string SendInfo, string EncodedToken);
    }
    public class MailSender : ISender
    {
        public async Task<SigningResult> SendCode(string EncodedEmail, string EncodedToken)
        {
            var reader = new AppSettingsReader();

            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            string decodedEmail = HttpUtility.UrlDecode(EncodedEmail);
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = "Email Verification";
            //here we will add code into html code
            mailMessage.From = new MailAddress((string)reader.GetValue("Email",typeof(string)));
            mailMessage.To.Add(new MailAddress(decodedEmail));
            mailMessage.Body = "";
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential((string)reader.GetValue("Email",typeof(string)), (string)reader.GetValue("EmailVerificationCode", typeof(string)));
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = nc;
            smtpClient.Send(mailMessage);
           
            return SigningResult.Success;
        }
    }
    public class SmsSender : ISender
    {
        public async Task<SigningResult> SendCode(string EncodedPhoneNumber, string EncodedToken)
        {
            throw new NotImplementedException();
        }
    }
}
