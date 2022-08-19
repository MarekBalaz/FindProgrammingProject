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
        CredentialsNotSet,
        Error
    }
    public interface ISignClass
    {
        Task<string> SignIn(string Email, string Password);
        //Task<SigningResult> SignOut();
        Task<SigningResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation);
        Task<string> ThirdPartySignIn(ExternalLoginInfo result);
    }

    public class SignClass : ISignClass
    {
        public UserManager<User> userManager;
        public SignInManager<User> signInManager;
        private ICodeGenerator codeGenerator;
        private ICreation creation;
        private IJwtTokenGenerator jwtTokenGenerator;
        public SignClass(UserManager<User> _userManager, SignInManager<User> _signInManager, IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            codeGenerator = new EmailVerificationCodeGenerator(_userManager, new MailSender(configuration));
            creation = new Creation(_userManager);
            this.jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<string> SignIn(string Email, string Password)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return SigningResult.EmailNotFound.ToString();
            }
            var result = await userManager.CheckPasswordAsync(user, Password);  
            if (result)
            {
                return jwtTokenGenerator.GetJwtToken(user);
            }
            else
            {
                if(user.EmailConfirmed == false)
                {
                    return SigningResult.EmailNotVerified.ToString();
                }
                else if(await userManager.IsLockedOutAsync(user))
                {
                    return SigningResult.AccountLockedOut.ToString();
                }
                return SigningResult.IncorrectPassword.ToString();
            } 
        }
        //public async Task<SigningResult> SignOut()
        //{
        //    await signInManager.SignOutAsync();
        //    return SigningResult.Success;
        //}
        public async Task<SigningResult> SignUp(string Email, string Nickname, string Password, string PasswordConfirmation)
        {
            if (Password == PasswordConfirmation)
            {
                
                var result = await userManager.FindByEmailAsync(Email);
                
                if (result == null)
                {
                    //Here we will generate email confirmation code and send it to email
                    //We will need to set event in database to clear all unconfirmed accounts after 30 minutes
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
        public async Task<string> ThirdPartySignIn(ExternalLoginInfo result)
        {
            if (result == null)
            {
                return SigningResult.DataDidNotCome.ToString();
            }
            var loginResult = await signInManager.ExternalLoginSignInAsync(result.LoginProvider,result.ProviderKey,true,true);
            if(loginResult.Succeeded)
            {
                var email = result.Principal.Claims.First(x => x.Type == ClaimTypes.Email).Value;
                var user = await userManager.FindByEmailAsync(email);
                return jwtTokenGenerator.GetJwtToken(user);
            }
            else if(loginResult.IsLockedOut)
            {
                return SigningResult.AccountLockedOut.ToString();
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
                    return jwtTokenGenerator.GetJwtToken(newUser);
                }
                else
                {
                    //add user external login and sign him in
                    await userManager.AddLoginAsync(user, result);
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    return jwtTokenGenerator.GetJwtToken(user);

                }
                
            }

            
        }
    }

}
