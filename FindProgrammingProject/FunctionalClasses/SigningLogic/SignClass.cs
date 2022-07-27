using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{

    public enum SigningResult
    {
        Success,
        IncorrectPassword,
        EmailNotVerified,
        AccountLockedOut,
        EmailNotFound,
        PasswordsDoNotMatch,
        EmailIsAlreadyRegistered,
        EmailIncorrect,
        DataDidNotCome,
        IncorrectToken,
        Error
    }
    public interface ISignClass
    {
        Task<SigningResult> SignIn(string Email, string Password);
        Task<SigningResult> SignOut();
        Task<SigningResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation);
        Task<SigningResult> ThirdPartySignIn(ExternalLoginInfo result);
    }

    public class SignClass : ISignClass
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ICodeGenerator codeGenerator;
        private ICreation creation;
        public SignClass(UserManager<User> _userManager, SignInManager<User> _signInManager, ICreation _creation)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            codeGenerator = new EmailVerificationCodeGenerator(_userManager,new MailSender());
            creation = _creation;
        }
        public async Task<SigningResult> SignIn(string Email, string Password)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return SigningResult.EmailNotFound;
            }
            var result = await signInManager.PasswordSignInAsync(Email, Password, true, false);
            if (result.Succeeded)
            {
                return SigningResult.Success;
            }
            else if (result.IsLockedOut)
            {
                return SigningResult.AccountLockedOut;
            }
            else if (user.EmailConfirmed == false)
            {
                return SigningResult.EmailNotVerified;
            }
            else
            {
                return SigningResult.IncorrectPassword;
            }
        }
        public async Task<SigningResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return SigningResult.Success;
        }
        public async Task<SigningResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation)
        {
            if (Password == PasswordConfirmation)
            {
                var result = await userManager.FindByEmailAsync(Email);
                if (result == null)
                {
                    //Here we will generate email confirmation code and send it to email
                    //We will need to set event in database to clear all unconfirmed accounts after 15 minutes
                    User user = await creation.Create(Email, Password, Nickname);

                    return await codeGenerator.GenerateCode(user);
                }
                else
                {
                    //Here we will return email already exist
                    return SigningResult.EmailIsAlreadyRegistered;
                }
            }
            else
            {
                return SigningResult.PasswordsDoNotMatch;
            }

        }
        public async Task<SigningResult> ThirdPartySignIn(ExternalLoginInfo result)
        {
            if (result == null)
            {
                return SigningResult.DataDidNotCome;
            }
            var loginResult = await signInManager.ExternalLoginSignInAsync(result.LoginProvider,result.ProviderKey,true,true);
            if(loginResult.Succeeded)
            {
                return SigningResult.Success;
            }
            else if(loginResult.IsLockedOut)
            {
                return SigningResult.AccountLockedOut;
            }
            else
            {
                var email = result.Principal.Claims.First(x => x.Type == ClaimTypes.Email).Value;
                var user = await userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    //create new user and add him external login and sign him in
                    var newUser = new User { Email = email };
                    await userManager.CreateAsync(newUser);
                    await userManager.AddLoginAsync(newUser, result);
                    newUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(newUser);
                    await signInManager.SignInAsync(newUser, true);
                }
                else
                {
                    //add user external login and sign him in
                    await userManager.AddLoginAsync(user, result);
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await signInManager.SignInAsync(user,true);
                    
                }
            }

            return SigningResult.Success;
        }
    }

}
