using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{

    public interface ISender
    {
        public Task<SigningResult> SendCode(string SendInfo, string EncodedToken, bool IsPasswordResetToken);
    }
    public class MailSender : ISender
    {
        public IConfiguration Configuration { get; set; }
        public MailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<SigningResult> SendCode(string Email, string Token, bool IsPasswordResetToken)
        {

            Token = HttpUtility.UrlEncode(Encryptor.EncryptString(Configuration.GetValue<string>("EncryptionKey"), Token));
            Email = HttpUtility.UrlEncode(Email);
            string url = "";
            string message1 = "";
            string message2 = "";
            string message3 = "";
            if(IsPasswordResetToken == true)
            {
                url = $"https://localhost:7047/SignIn/VerifyPasswordResetCode/?Email={Email}&Token={Token}";
                message1 = "You are receiving this email because we received a password reset request for your account. To proceed with the password reset please click on the button below:";
                message2 = "This password reset link will expire in 15 minutes. If you did not request a password reset, no further action is required.";
                message3 = "Reset Password";
            }
            else
            {
                url = $"https://localhost:7047/SignIn/VerifyEmailVerificationCode/?Email={Email}&Token={Token}";
                message1 = "To activate your account, please click on the button below to verify your email address. Once activated, you’ll have access to our products.";
                message2 = "This email verification link will expire in 15 minutes. If you did not request a password reset, no further action is required.";
                message3 = "Activate Account";
            }
            string htmlContent = @$"<style>html,body {{ padding: 0; margin:0; }}</style>
                                    <div style=""font-family:Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: 15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#edf2f7"">
	                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse:collapse;margin:0 auto; padding:0; max-width:600px"">
		                                    <tbody>
			                                    <tr>
				                                    <td align=""center"" valign=""center"" style=""text-align:center; padding: 40px"">
					                                    <a href=""https://fpp.com"" rel=""noopener"" target=""_blank"">
						                                    <img alt=""Logo"" src=""../../assets/media/logos/mail.svg"" />
					                                    </a>
				                                    </td>
			                                    </tr>
			                                    <tr>
				                                    <td align=""left"" valign=""center"">
					                                    <div style=""text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px"">
						                                    <!--begin:Email content-->
						                                    <div style=""padding-bottom: 30px; font-size: 17px;"">
							                                    <strong>Welcome to FPP!</strong>
						                                    </div>
						                                    <div style=""padding-bottom: 30px"">{message1}</div>
						                                    <div style=""padding-bottom: 40px; text-align:center;"">
							                                    <a href=""{url}"" rel=""noopener"" style=""text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:blue;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">{message3}</a>
						                                    </div>
						                                    <div style=""padding-bottom: 30px"">{message2}</div>
						                                    <div style=""border-bottom: 1px solid #eeeeee; margin: 15px 0""></div>
						                                    <div style=""padding-bottom: 50px; word-wrap: break-all;"">
							                                    <p style=""margin-bottom: 10px;"">Button not working? Try pasting this URL into your browser:</p>
							                                    <a href=""{url}"" rel=""noopener"" target=""_blank"" style=""text-decoration:none;color:rgb(35, 35, 245)"">{url}</a>
						                                    </div>
						                                    <!--end:Email content-->
						                                    <div style=""padding-bottom: 10px"">Kind regards,
						                                    <br>The FPP Team.
						                                    <tr>
							                                    <td align=""center"" valign=""center"" style=""font-size: 13px; text-align:center;padding: 20px; color: #6d6e7c;"">
								                                    <p>Floor 5, 450 Avenue of the Red Field, SF, 10050, USA.</p>
								                                    <p>Copyright &copy;
								                                    <a href=""https://fpp.com"" rel=""noopener"" target=""_blank"">FPP</a>.</p>
							                                    </td>
						                                    </tr></br></div>
					                                    </div>
				                                    </td>
			                                    </tr>
		                                    </tbody>
	                                    </table>
                                    </div> ";

            string decodedEmail = HttpUtility.UrlDecode(Email);
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = "Email Verification for FPP";
            //here we will add code into html code
            mailMessage.From = new MailAddress(Configuration.GetValue<string>("Email"));
            try
            {
                mailMessage.To.Add(new MailAddress(decodedEmail));
            }
            catch
            {
                return SigningResult.EmailIncorrect;
            }
            mailMessage.Body = htmlContent;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(Configuration.GetValue<string>("Email"), Configuration.GetValue<string>("EmailVerificationCode"));
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = nc;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                return SigningResult.EmailIncorrect;
            }
            return SigningResult.Success;
        }
    }
    public class SmsSender : ISender
    {
        public async Task<SigningResult> SendCode(string EncodedPhoneNumber, string EncodedToken, bool IsPasswordResetToken)
        {
            throw new NotImplementedException();
        }
    }
}
