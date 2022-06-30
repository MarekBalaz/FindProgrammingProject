using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;

namespace FindProgrammingProject.FunctionalClasses
{
    
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
        EmailIncorrect
    }

    public interface ISignClass
    {
        Task<SignInResult> SignIn(string Email, string Password);
        Task<SignInResult> SignOut();
        Task<SignUpResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation);
        Task<int> ThirdPartySignIn();
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
        public async Task<int> ThirdPartySignIn()
        {
            return 1;
        }
    }

}
