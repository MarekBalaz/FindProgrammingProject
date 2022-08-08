
using FindProgrammingProject.FunctionalClasses.SigningLogic;
﻿using FindProgrammingProject.FunctionalClasses;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FindProgrammingProject.Controllers
{
    [ApiController]
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
        public SigningController(ISignClass signClass, UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<SigningController> logger, IConfiguration configuration)
        {
            this.signClass = signClass;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.configuration = configuration;
        }
        //This action will either sign in person or return sign in view
        [HttpGet]
        [Route("signing/signin")]
        public async Task<string> SignIn(string Email = "", string Password = "", string ReturnUrl = "Home/Index")
        {
            try
            {
                if (Email == "" && Password == "")
                {
                    return SigningResult.CredentialsNotSet.ToString();
                }
                else
                {
                    return await signClass.SignIn(Email, Password);
                }
            }
            catch(Exception ex)
            {
                var jsonResponse = JsonSerializer.Serialize(ex);
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error,ex,null);
                return jsonResponse;
            }
            
        }
        //This endpoint will not do anything because log out will be done by removing sessions on web server
        [HttpGet]
        [Route("signing/signout")]
        public void LogOut()
        {
            //await signClass.SignOut();
            //return SigningResult.Success.ToString();
        }
        //This action will either sign up person or return sign up view
        [HttpPost]
        [Route("signing/singup")]
        public async Task<string> SignUp(string Email, string Password, string PasswordConfirmation, string Nickname)
        {
            try
            {
                if (Email == "" || Password == "" || PasswordConfirmation == "" || Nickname == "")
                {
                    return SigningResult.CredentialsNotSet.ToString();
                }
                var trimmedEmail = Email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    return SigningResult.EmailNotFound.ToString();
                }
                try
                {
                    var addr = new System.Net.Mail.MailAddress(Email);
                    if(addr.Address == trimmedEmail)
                    {

                    }
                }
                catch
                {
                    return @"{""Message"":""Email is invalid""}"; ;
                }
                SigningResult signUpResult = await signClass.SignUp(Email, Nickname, Password, PasswordConfirmation);
                if (signUpResult == SigningResult.Success)
                {
                    var result = await signClass.SignIn(Email, Password);
                    return result;
                }
                return signUpResult.ToString();
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(await userManager.FindByEmailAsync(Email));
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("signing/getauthorizationtoken")]
        public async Task<string> GetAuthorizationToken(string Email)
        {
            JwtTokenGenerator tokenGenerator = new JwtTokenGenerator();
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                string token = tokenGenerator.GetJwtToken(user);
                return token;
            }
            return SigningResult.EmailNotFound.ToString();
        }
        [HttpGet]
        [Route("signing/verifyauthorizationtoken")]
        public async Task<string> VerifyAuthorizationToken(string Email, string Token)
        {
            if(Email != "")
            {
                verification = new AuthorizationTokenVerification();
                SigningResult result = await verification.Verify(Email, Token);

                return result.ToString();
            }
            return SigningResult.EmailNotFound.ToString();
        }
        [HttpGet]
        [Route("signing/sendresetpasswordcode")]
        public async Task<string> SendResetPasswordCode(string Email)
        {
            try
            {
                sender = new MailSender(configuration);
                codeGenerator = new PasswordResetCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);
                    
                    return result.ToString();
                }
                return SigningResult.EmailNotFound.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("signing/getresetpasswordcode")]
        public async Task<string> GetResetPasswordCode(string Email)
        {
            try
            {
                //sender = new MailSender(configuration);
                codeGenerator = new PasswordResetCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);

                    return result;
                }
                return SigningResult.EmailNotFound.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("singing/verifyresetpasswordtoken")]
        public async Task<string> VerifyResetPasswordToken(string email, string token)
        {
            try
            {
                if(email == "" || token == "")
                {
                    return SigningResult.CredentialsNotSet.ToString();
                }
                verification = new PasswordResetTokenVerifiction(userManager);
                SigningResult result = await verification.Verify(email, token);
                if (result == SigningResult.Success)
                {
                    return SigningResult.Success.ToString();
                }
                return SigningResult.IncorrectToken.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpPost]
        [Route("signing/setnewpassword")]
        public async Task<string> SetNewPassword(string newPassword, string newPasswordRepeated,string token, string email)
        {
            try
            {
                if(newPassword == ""|| newPasswordRepeated == "" || token == "" || email == "")
                {
                    return SigningResult.CredentialsNotSet.ToString();
                }
                reset = new ResetPassword(userManager, new PasswordResetTokenVerifiction(userManager));
                var result = await reset.Reset(newPassword, newPasswordRepeated, token, email);
                return result.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("singing/sendemailverificationcode")]
        public async Task<string> SendEmailVerificationCode(string Email)
        {
            try
            {
                sender = new MailSender(configuration);
                codeGenerator = new EmailVerificationCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);
                    
                    return result.ToString();
                }
                return SigningResult.EmailNotFound.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("singing/sendemailverificationcode")]
        public async Task<string> GetEmailVerificationCode(string Email)
        {
            try
            {
                codeGenerator = new EmailVerificationCodeGenerator(userManager, null);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    string result = await codeGenerator.GetCode(user);
                    
                    return result;
                }
                return SigningResult.EmailNotFound.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("singing/verifyemail")]
        public async Task<string> VerifyEmail(string Email, string Token)
        {
            try
            {
                if(Email == "" || Token == "")
                {
                    return SigningResult.CredentialsNotSet.ToString();
                }
                verification = new EmailTokenVerification(userManager, signInManager);
                SigningResult result = await verification.Verify(Email, Token);
                return result.ToString();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }       
        //public IActionResult SendThirdPartySignIn(string provider)
        //{
        //    var options = signInManager.ConfigureExternalAuthenticationProperties(provider, "ThirdPartySignIn");
        //    return Challenge(options, provider);
        //}
        [HttpGet]
        [Route("signing/thirdpartysignin")]
        public async Task<string> ThirdPartySignIn(string externalLoginInfo)
        {
            try
            {
                var result = JsonSerializer.Deserialize<ExternalLoginInfo>(externalLoginInfo);
                var response = await signClass.ThirdPartySignIn(result);
                return response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
    }
}
