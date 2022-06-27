using FindProgrammingProject.FunctionalClasses;
using Microsoft.AspNetCore.Mvc;

namespace FindProgrammingProject.Controllers
{
    public class SigningController : Controller
    {
        private ISignClass signClass;
        public SigningController(ISignClass _signClass)
        {
            signClass = _signClass;
        }
        //This action will either sign in person or return sign in view
        public IActionResult SignIn(string Email = "", string Password = "", string ReturnUrl = "Home/Index")
        {
            if(Email == "" && Password == "")
            {
                ViewBag.Message = "Return of SignIn view";
                return View("SignIn");
            }
            else if(Email == "" || Password == "")
            {
                ViewBag.Message = "Credentials were not set";
                return View("SignIn");
            }
            else
            {

                return View(ReturnUrl);
            }
        }
        //This action will either sign out person or return sign out view
        public IActionResult SignOut()
        {
            return View();
        }
        //This action will either sign up person or return sign up view
        public IActionResult SignUp()
        {
            return View();
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
        public IActionResult VerifyEmail()
        {
            return View();
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
