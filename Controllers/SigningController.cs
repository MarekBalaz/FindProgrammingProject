using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.FunctionalClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using FindProgrammingProject.Models.ObjectModels;

namespace FindProgrammingProject.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class SigningController : ControllerBase
    {
        private ISignClass signClass;
        private IVerification? verification;
        private ICodeGenerator? codeGenerator;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IReset? reset;
        private ISender? sender;
        private ILogger<SigningController> logger;
        private IConfiguration configuration;
        private ICreation creation;
        public SigningController(UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<SigningController> logger, IConfiguration configuration, IJwtTokenGenerator jwtTokenGenerator)
        {
            this.signClass = new SignClass(userManager, signInManager, jwtTokenGenerator, configuration);
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.configuration = configuration;
        }
        //This action will either sign in person or return sign in view
        [HttpPost]
        [Route("signin")]
        public async Task<string> SignIn([FromHeader]string Email, [FromHeader]string Password)
        {
            try
            {
                string jsonResponse;
                verification = new AuthorizationTokenVerification(configuration);
                if (Email == "" && Password == "")
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.CredentialsNotSet}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                else
                {
                    var response = await signClass.SignIn(Email, Password);
                    if(response.Substring(0,2) != "ey")
                    {
                        jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"false\"}}";
                        return jsonResponse;
                    }
                    if(await verification.Verify(Email, response) == SigningResult.Success)
                    {
                        jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"true\"}}";
                        return jsonResponse;
                    }
                    jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
            }
            catch (Exception ex)
            {
                var jsonResponse = JsonSerializer.Serialize(ex);
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return jsonResponse;
            }

        }
        //This action will either sign up person or return sign up view
        [HttpPost]
        [Route("signup")]
        public async Task<string> SignUp([FromHeader]string Email = "", [FromHeader] string Password = "", [FromHeader] string PasswordConfirmation = "", [FromHeader]string Nickname = "")
        {
            try
            {
                verification = new AuthorizationTokenVerification(configuration);
                string jsonResponse;
                Email = HttpUtility.UrlDecode(Email);
                Password = HttpUtility.UrlDecode(Password);
                PasswordConfirmation = HttpUtility.UrlDecode(PasswordConfirmation);
                Nickname = HttpUtility.UrlDecode(Nickname);
                if (Email == "" || Password == "" || PasswordConfirmation == "" || Nickname == "")
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.CredentialsNotSet}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                var trimmedEmail = Email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.EmailIncorrect}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                try
                {
                    var addr = new System.Net.Mail.MailAddress(Email);
                    if (addr.Address == trimmedEmail)
                    {

                    }
                }
                catch
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.EmailIncorrect}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                SigningResult signUpResult = await signClass.SignUp(Email, Nickname, Password, PasswordConfirmation);
                if (signUpResult == SigningResult.Success)
                {
                    var result = await signClass.SignIn(Email, Password);
                    if(await verification.Verify(Email, result) == SigningResult.Success)
                    {
                        jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"true\"}}";
                        return jsonResponse;
                    }
                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{signUpResult}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                }
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("getauthorizationtoken")]
        private async Task<string> GetAuthorizationToken(string Email)
        {
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                JwtTokenGenerator tokenGenerator = new JwtTokenGenerator();
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string token = tokenGenerator.GetJwtToken(user);
                    string tokenResponse = $"{{\"Message\":\"{token}\", \"Token\":\"true\"}}";
                    return tokenResponse;
                }
                string jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("verifyauthorizationtoken")]
        public async Task<string> VerifyAuthorizationToken([FromHeader] string Email, [FromHeader] string Token)
        {
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                Token = HttpUtility.UrlDecode(Token);
                string jsonResponse;
                if (Email != "")
                {
                    verification = new AuthorizationTokenVerification(configuration);
                    SigningResult result = await verification.Verify(Email, Token);
                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                    
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("sendresetpasswordcode")]
        public async Task<string> SendResetPasswordCode(string Email)
        {
            try
            {
                Email = HttpUtility.UrlDecode(Email);
                sender = new MailSender(configuration);
                string jsonResponse;
                codeGenerator = new PasswordResetCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);

                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("getresetpasswordcode")]
        private async Task<string> GetResetPasswordCode(string Email)
        {
            try
            {
                string jsonResponse;
                Email = HttpUtility.UrlDecode(Email);
                //sender = new MailSender(configuration);
                codeGenerator = new PasswordResetCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);

                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("verifyresetpasswordtoken")]
        public async Task<string> VerifyResetPasswordToken([FromHeader] string Email = "", [FromHeader] string Token = "")
        {
            try
            {
                string jsonResponse;
                if (Email == "" || Token == "")
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.CredentialsNotSet}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                verification = new PasswordResetTokenVerifiction(userManager);
                SigningResult result = await verification.Verify(Email, Token);
                if (result == SigningResult.Success)
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.Success}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.IncorrectToken}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("setnewpassword")]
        public async Task<string> SetNewPassword([FromHeader] string NewPassword, [FromHeader] string NewPasswordConfirmed, [FromHeader] string Token, [FromHeader] string Email)
        {
            try
            {
                string jsonResponse;
                if (NewPassword == "" || NewPasswordConfirmed == "" || Token == "" || Email == "")
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.CredentialsNotSet}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                reset = new ResetPassword(userManager, new PasswordResetTokenVerifiction(userManager));
                var result = await reset.Reset(NewPassword, NewPasswordConfirmed, Token, Email);
                jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("sendemailverificationcode")]
        public async Task<string> SendEmailVerificationCode(string Email)
        {
            try
            {
                string jsonResponse;
                Email = HttpUtility.UrlDecode(Email);
                sender = new MailSender(configuration);
                codeGenerator = new EmailVerificationCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);

                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("getemailverificationcode")]
        private async Task<string> GetEmailVerificationCode(string Email)
        {
            try
            {
                string jsonResponse;
                codeGenerator = new EmailVerificationCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);
                    result = HttpUtility.UrlEncode(result);
                    jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{SigningResult.EmailNotFound}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
        
        [HttpPost]
        [Route("verifyemail")]
        public async Task<string> VerifyEmail([FromHeader] string Email, [FromHeader] string Token)
        {
            try
            {
                string jsonResponse;
                if(Email == "" || Token == "")
                {
                    jsonResponse = $"{{\"Message\":\"{SigningResult.CredentialsNotSet}\", \"Token\":\"false\"}}";
                    return jsonResponse;
                }
                verification = new EmailTokenVerification(userManager, signInManager);
                SigningResult result = await verification.Verify(Email, Token);
                jsonResponse = $"{{\"Message\":\"{result}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }       
        [HttpPost]
        [Route("thirdpartysignin")]
        public async Task<string> ThirdPartySignIn([FromHeader] string LoginProvider, [FromHeader] string ProviderKey, [FromHeader] string Email, [FromHeader] string ProviderDisplayName)
        {
            try
            {
                verification = new AuthorizationTokenVerification(configuration);
                UserLoginInfo lg = new UserLoginInfo(LoginProvider,ProviderKey, ProviderDisplayName);
                string jsonResponse;
                var response = await signClass.ThirdPartySignIn(LoginProvider, ProviderKey, Email, lg);
                if(await verification.Verify(Email, response) == SigningResult.Success)
                {
                    jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"true\"}}";
                    return jsonResponse;
                }
                jsonResponse = $"{{\"Message\":\"{response}\", \"Token\":\"false\"}}";
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, ex.Message);
                return JsonSerializer.Serialize(ex);
            }
        }
    }
}
