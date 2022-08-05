
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
        public SigningController(ISignClass signClass, UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<SigningController> logger)
        {
            this.signClass = signClass;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
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
                    return @"{""Message"":""Credentials were not set""}";
                }
                else if (Email == "")
                {
                    return @"{""Message"":""Email was not set""}";
                }
                else if (Password == "")
                {
                    return @"{""Message"":""Password was not set""}";
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
                SigningResult signUpResult = await signClass.SignUp(Email, Nickname, Password, PasswordConfirmation);
                if (signUpResult == SigningResult.Success)
                {
                    var result = await signClass.SignIn(Email, Password);
                    return result;
                }
                return @"{""Message"":""We have sent you a verification code to your email. Please verify your email.""}";
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                logger.Log(LogLevel.Error, ex, null);
                return JsonSerializer.Serialize(ex);
            }
        }
        [HttpGet]
        [Route("signing/sendresetpasswordcode")]
        public async Task<string> SendResetPasswordCode(string Email)
        {
            try
            {
                sender = new MailSender();
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
        [Route("singing/verifyresetpasswordtoken")]
        public async Task<string> VerifyResetPasswordToken(string email, string token)
        {
            try
            {
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
                sender = new MailSender();
                codeGenerator = new EmailVerificationCodeGenerator(userManager, sender);
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    SigningResult result = await codeGenerator.GenerateCode(user);
                    if (result == SigningResult.Success)
                    {
                        return SigningResult.Success.ToString();
                    }
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
        [Route("singing/verifyemail")]
        public async Task<string> VerifyEmail(string Email, string Token)
        {
            try
            {
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
        public async Task<string> ThirdPartySignIn(ExternalLoginInfo result)
        {
            try
            {
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
