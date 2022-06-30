using FindProgrammingProject.FunctionalClasses;
using FindProgrammingProject.FunctionalClasses.SigningLogic;
using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class SigningController : Controller
    {
        private ISignClass signClass;
        private IVerification verification;
        public SigningController(ISignClass signClass, IVerification verification)
        {
            this.signClass = signClass;
            this.verification = verification;
        }
        //This action will either sign in person or return sign in view
        public async Task<IActionResult> SignIn(string Email = "", string Password = "", string ReturnUrl = "Home/Index")
        {
            if(Email == "" && Password == "")
            {
                ViewBag.Message = "Credentials were not set";
                return View("SignIn");
            }
            else if(Email == "" || Password == "")
            {
                ViewBag.Message = "Credentials were not set";
                return View("SignIn");
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
        public async Task<IActionResult> SignOut()
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
        public IActionResult SendResetPasswordCode()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult SendEmailVerificationCode()
        {
            return View();
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
        public IActionResult SendThirdPartySignIn()
        {
            return new ChallengeResult();
        }
        public IActionResult ThirdPartySignIn()
        {
            return View();
        }
    }
}
