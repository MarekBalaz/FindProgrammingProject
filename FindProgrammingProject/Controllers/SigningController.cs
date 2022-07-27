
using FindProgrammingProject.FunctionalClasses.SigningLogic;
﻿using FindProgrammingProject.FunctionalClasses;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public SigningController(ISignClass signClass, UserManager<User> userManager,
            SignInManager<User> signInManager) 
        {
            this.signClass = signClass;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        //This action will either sign in person or return sign in view
        [HttpGet]
        [Route("signing/signin")]
        public async Task<string> SignIn(string Email = "", string Password = "", string ReturnUrl = "Home/Index")
        {
            if(Email == "" && Password == "")
            {
                
                return "Credentials were not set";
            }
            else if(Email == "" || Password == "")
            {
                return "Credentials were not set";
            }
            else
            {
                SigningResult result = await signClass.SignIn(Email,Password);
                if (result == SigningResult.Success)
                {
                    return SigningResult.Success.ToString();
                }
                else
                {
                    return result.ToString();
                }
            }
        }
        //This action will either sign out person or return sign out view
        //This method had to bee renamed because ControllerBase class contains method with the same name
        [HttpGet]
        [Route("signing/signout")]
        public async Task<string> LogOut()
        {
            await signClass.SignOut();
            return SigningResult.Success.ToString();
        }
        //This action will either sign up person or return sign up view
        [HttpPost]
        [Route("signing/singup")]
        public async Task<string> SignUp(string Email, string Password, string PasswordConfirmation, string Nickname)
        {
            SigningResult result = await signClass.SignUp(Email,Nickname,Password,PasswordConfirmation);
            if(result == SigningResult.Success)
            {
                return SigningResult.Success.ToString();
            }
            return result.ToString();
        }
        [HttpGet]
        [Route("signing/sendresetpasswordcode")]
        public async Task<string> SendResetPasswordCode(string Email)
        {
            sender = new MailSender();
            codeGenerator = new PasswordResetCodeGenerator(userManager, sender);
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                SigningResult result = await codeGenerator.GenerateCode(user);
                if (result == SigningResult.Success)
                {
                    return result.ToString();
                }
                return result.ToString();
            }
            return SigningResult.EmailNotFound.ToString();
        }
        [HttpGet]
        [Route("singing/verifyresetpasswordtoken")]
        public async Task<string> VerifyResetPasswordToken(string email, string token)
        {
            verification = new PasswordResetTokenVerifiction(userManager);
            SigningResult result = await verification.Verify(email, token);
            if(result == SigningResult.Success)
            {                   
                return SigningResult.Success.ToString();
            }
            return SigningResult.IncorrectToken.ToString();
        }
        [HttpPost]
        [Route("signing/setnewpassword")]
        public async Task<string> SetNewPassword(string newPassword, string newPasswordRepeated,string token, string email)
        {
            reset = new ResetPassword(userManager,new PasswordResetTokenVerifiction(userManager));
            var result = await reset.Reset(newPassword,newPasswordRepeated,token,email);
            if(result == SigningResult.Success)
            {
                return SigningResult.Success.ToString();
            }
            return result.ToString();
        }
        [HttpGet]
        [Route("singing/sendemailverificationcode")]
        public async Task<string> SendEmailVerificationCode(string Email)
        {
            sender = new MailSender();
            codeGenerator = new EmailVerificationCodeGenerator(userManager,sender);
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
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
        [HttpGet]
        [Route("singing/verifyemail")]
        public async Task<string> VerifyEmail(string Email, string Token)
        {
            verification = new EmailTokenVerification(userManager,signInManager);
            SigningResult result = await verification.Verify(Email, Token);
            if(result == SigningResult.Success)
            {
                return SigningResult.Success.ToString();
            }
            return result.ToString();
        }       
        public IActionResult SendThirdPartySignIn(string provider)
        {
            var options = signInManager.ConfigureExternalAuthenticationProperties(provider, "ThirdPartySignIn");
            return Challenge(options, provider);
        }
        [HttpGet]
        [Route("signing/thirdpartysignin")]
        public async Task<string> ThirdPartySignIn()
        {
            var result = await signInManager.GetExternalLoginInfoAsync();
            var response = await signClass.ThirdPartySignIn(result);
            if(response == SigningResult.Success)
            {
                return SigningResult.Success.ToString();
            }
            return response.ToString();
        }
    }
}
