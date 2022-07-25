using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
<<<<<<< HEAD

=======
    public enum ExternalLoginResponse
    {
        Success,
        DataDidNotCome,
        AccountLockedOut
    }
>>>>>>> 43d50c8f43db04dfc2f96b8a254aaee84f8a1290
    public enum SignInResult
    {
        Success,
        IncorrectPassword,
        EmailNotVerified,
        AccountLockedOut,
        EmailNotFound
    }
    public enum SignUpResult
    {
        Success,
        PasswordsDoNotMatch,
        EmailIsAlreadyRegistered,
        EmailIncorrect,
        Error
    }

    public interface ISignClass
    {
        Task<SignInResult> SignIn(string Email, string Password);
        Task<SignInResult> SignOut();
        Task<SignUpResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation);
        Task<ExternalLoginResponse> ThirdPartySignIn(ExternalLoginInfo result);
    }

    public class SignClass : ISignClass
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ICodeGenerator codeGenerator;
        private ICreation creation;
        public SignClass(UserManager<User> _userManager, SignInManager<User> _signInManager, ICodeGenerator _codeGenerator, ICreation _creation)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            codeGenerator = _codeGenerator;
            creation = _creation;
        }
        public async Task<SignInResult> SignIn(string Email, string Password)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return SignInResult.EmailNotFound;
            }
            var result = await signInManager.PasswordSignInAsync(Email, Password, true, false);
            if (result.Succeeded)
            {
                return SignInResult.Success;
            }
            else if (result.IsLockedOut)
            {
                return SignInResult.AccountLockedOut;
            }
            else if (user.EmailConfirmed == false)
            {
                return SignInResult.EmailNotVerified;
            }
            else
            {
                return SignInResult.IncorrectPassword;
            }
        }
        public async Task<SignInResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return SignInResult.Success;
        }
        public async Task<SignUpResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation)
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
                    return SignUpResult.EmailIsAlreadyRegistered;
                }
            }
            else
            {
                return SignUpResult.PasswordsDoNotMatch;
            }

        }
        public async Task<ExternalLoginResponse> ThirdPartySignIn(ExternalLoginInfo result)
        {
            if (result == null)
            {
                return ExternalLoginResponse.DataDidNotCome;
            }
            var loginResult = await signInManager.ExternalLoginSignInAsync(result.LoginProvider,result.ProviderKey,true,true);
            if(loginResult.Succeeded)
            {
                return ExternalLoginResponse.Success;
            }
            else if(loginResult.IsLockedOut)
            {
                return ExternalLoginResponse.AccountLockedOut;
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

            return ExternalLoginResponse.Success;
        }
    }

}
