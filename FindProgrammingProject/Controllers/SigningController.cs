using FindProgrammingProject.FunctionalClasses;
using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class SigningController : Controller
    {
        private ISignClass signClass;
        private IVerification verification;
        private ICodeGenerator codeGenerator;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IReset reset;
        public SigningController(ISignClass signClass, IVerification verification,
            ICodeGenerator codeGenerator, UserManager<User> userManager, IReset reset,
            SignInManager<User> signInManager) 
        {
            this.signClass = signClass;
            this.verification = verification;
            this.codeGenerator = codeGenerator;
            this.userManager = userManager;
            this.reset = reset;
            this.signInManager = signInManager;
        }
        //This action will either sign in person or return sign in view
        public async Task<IActionResult> SignIn(string Email = "", string Password = "", string ReturnUrl = "Home/Index")
        {
            if(Email == "" && Password == "")
            {
                ViewBag.Message = "Credentials were not set";
                return View("FPP-Signin");
            }
            else if(Email == "" || Password == "")
            {
                ViewBag.Message = "Credentials were not set";
                return View("FPP-Signin");
            }
            else
            {
                FunctionalClasses.SignInResult result = await signClass.SignIn(Email,Password);
                if (result == FunctionalClasses.SignInResult.Success)
                {
                    return View(ReturnUrl);
                }
                else
                {
                    ViewBag.Message = result;
                    return View("FPP-Signin");
                }
            }
        }
        //This action will either sign out person or return sign out view
        //This method had to bee renamed because ControllerBase class contains method with the same name
        public async Task<IActionResult> LogOut()
        {
            await signClass.SignOut();
            ViewBag.Message = "You were signed out succesfully";
            return View("InfoPage");
        }
        //This action will either sign up person or return sign up view
        public async Task<IActionResult> SignUp(string Email, string Password, string PasswordConfirmation, string Nickname)
        {
            SignUpResult result = await signClass.SignUp(Email,Nickname,Password,PasswordConfirmation);
            if(result == SignUpResult.Success)
            {
                ViewBag.Message = "We have sent you a verification code to your email. Please verify it within the next 15 minutes or your account will be deleted";
                return View("InfoPage");
            }
            ViewBag.Message = result;
            return View("InfoPage");
        }
        public async Task<IActionResult> SendResetPasswordCode(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                SignUpResult result = await codeGenerator.GenerateCode(user);
                if (result == SignUpResult.Success)
                {
                    ViewBag.Message = "We have sent you a verification code to your email. Please verify it within the next 15 minutes or your account will be deleted";
                    return View("InfoPage");
                }
                ViewBag.Message = result;
                return View("InfoPage");
            }
            ViewBag.Message = "User does not exist";
            return View("InfoPage");
        }
        public async Task<IActionResult> VerifyResetPasswordToken(string email, string token)
        {
            VerificationResult result = await verification.Verify(email, token);
            if(result == VerificationResult.Success)
            {                   
                return View("FPP-SetNewPassword");
            }
            ViewBag.Message = "Verification token was incorrect";
            return View("InfoPage");
        }
        public async Task<IActionResult> SetNewPassword(string newPassword, string newPasswordRepeated,string token, string email)
        {
            var result = await reset.Reset(newPassword,newPasswordRepeated,token,email);
            if(result == ResetResponse.Success)
            {
                ViewBag.Message = "Your password has been reset";
                return View("InfoPage");
            }
            ViewBag.Message = result;
            return View("InfoPage");
        }
        public async Task<IActionResult> SendEmailVerificationCode(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                SignUpResult result = await codeGenerator.GenerateCode(user);
                if (result == SignUpResult.Success)
                {
                    ViewBag.Message = "We have sent you a verification code to your email. Please verify it within the next 15 minutes or your account will be deleted";
                    return View("InfoPage");
                }
                ViewBag.Message = result;
                return View("InfoPage");
            }
            ViewBag.Message = "User was not found";
            return View("InfoPage");
        }
        public async Task<IActionResult> VerifyEmail(string Email, string Token)
        {
            VerificationResult result = await verification.Verify(Email, Token);
            if(result == VerificationResult.Success)
            {
                ViewBag.Message = result;
                return View("InfoPage");
            }
            ViewBag.Message = result;
            return View("InfoPage");
        }       
        public IActionResult SendThirdPartySignIn(string provider)
        {
            var options = signInManager.ConfigureExternalAuthenticationProperties(provider, "ThirdPartySignIn");
            return Challenge(options, provider);
        }
        public async Task<IActionResult> ThirdPartySignIn()
        {
            var result = await signInManager.GetExternalLoginInfoAsync();
            var response = await signClass.ThirdPartySignIn(result);
            if(response == ExternalLoginResponse.Success)
            {
                return View("Home/Index");
            }
            ViewBag.Message = response;
            return View("InfoPage");
        }
    }
}
