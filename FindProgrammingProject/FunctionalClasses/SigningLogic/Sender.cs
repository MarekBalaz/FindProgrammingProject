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
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            string decodedEmail = HttpUtility.UrlDecode(EncodedEmail);
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = "Email Verification";
            //here we will add code into html code
            mailMessage.From = new MailAddress("marekgamingacc@gmail.com");
            mailMessage.To.Add(new MailAddress(decodedEmail));
            mailMessage.Body = "";
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential("marekgamingacc@gmail.com", "nyfvvrvfineixyvt");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = nc;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                return SigningResult.EmailIncorrect;
            }
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
