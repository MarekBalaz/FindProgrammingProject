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
        public IConfiguration Configuration { get; set; }
        public MailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<SigningResult> SendCode(string EncodedEmail, string EncodedToken)
        {
            string htmlContent = @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta content=""text/html"" charset=""utf-8"" http-equiv=""Content-Type"">
<meta name=""viewport"" content=""width=device-width"">
  <title>replit</title>
<style type=""text/css"">
    * {
      color: black;
    }
  </style>
</head>

<body>
      <a href=""location"" style=""font-size: 2rem;text-align:center;"">Click here to confirm your email</a>
</body>

</html> ";

            string decodedEmail = HttpUtility.UrlDecode(EncodedEmail);
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = "Email Verification";
            //here we will add code into html code
            mailMessage.From = new MailAddress(Configuration.GetValue<string>("Email"));
            mailMessage.To.Add(new MailAddress(decodedEmail));
            mailMessage.Body = htmlContent;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(Configuration.GetValue<string>("Email"), Configuration.GetValue<string>("EmailVerificationCode"));
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
