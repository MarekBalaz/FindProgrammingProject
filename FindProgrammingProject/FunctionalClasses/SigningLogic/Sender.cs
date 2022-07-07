using System.Net;
using System.Net.Mail;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{

    public interface ISender
    {
        public Task<SignUpResult> SendCode(string SendInfo, string EncodedToken);
    }
    public class MailSender : ISender
    {
        public async Task<SignUpResult> SendCode(string EncodedEmail, string EncodedToken)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            MailMessage mailMessage = new MailMessage(appSettings["ApplicationInfo:EmailInfo:Email"], HttpUtility.UrlDecode(EncodedEmail));
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
    public class SmsSender : ISender
    {
        public async Task<SignUpResult> SendCode(string EncodedPhoneNumber, string EncodedToken)
        {
            throw new NotImplementedException();
        }
    }
}
