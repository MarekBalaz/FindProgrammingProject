using FindProgrammingProject.Models.ObjectModels;
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
        UserNameAlreadyRegistered,
        PasswordDoesNotFollowRules,
        InvalidAccessTokenOrRefreshToken,
        Error
    }
    public interface ISignClass
    {
        Task<string> SignIn(string Email, string Password);
        //Task<SigningResult> SignOut();
        Task<SigningResult> SignUp(string Email, string Nickname, string Password);
        Task<string> ThirdPartySignIn(string LoginProvider, string ProviderKey, string Email, UserLoginInfo info);
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
            if(user.EmailConfirmed == false)
            {
                return SigningResult.EmailNotVerified.ToString();
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
        public async Task<SigningResult> SignUp(string Email, string Nickname, string Password)
        {
            var result = await userManager.FindByEmailAsync(Email);
                
                if (result == null)
                {
                    //Here we will generate email confirmation code and send it to email
                    //We will need to set event in database to clear all unconfirmed accounts after 30 minutes
                    User user = await creation.Create(Email, Password, Nickname);
                    
                    if(user.UserName == "Exist" && user.Email == null)
                    {
                        return SigningResult.UserNameAlreadyRegistered;
                    }
                    else if(user.UserName != Nickname)
                    {
                        return SigningResult.PasswordDoesNotFollowRules;
                    }
                    return await codeGenerator.GenerateCode(user);
                }
                else
                {
                    //Here we will return email already exist
                    return SigningResult.EmailIsAlreadyRegistered;
                }
            

        }
        public async Task<string> ThirdPartySignIn(string LoginProvider, string ProviderKey, string Email, UserLoginInfo info)
        {
            if (ProviderKey == "")
            {
                return SigningResult.DataDidNotCome.ToString();
            }
            var user = await userManager.FindByLoginAsync(LoginProvider, ProviderKey);
            if (user != null)
            {
                return jwtTokenGenerator.GetJwtToken(user);
            }
            //var loginResult = await signInManager.ExternalLoginSignInAsync(LoginProvider,ProviderKey,true,true);
            else
            {
                user = await userManager.FindByEmailAsync(Email);
                if(user == null)
                {
                    //create new user and add him external login and sign him in
                    var newUser = new User { Email = Email };
                    await userManager.CreateAsync(newUser);
                    await userManager.AddLoginAsync(newUser, info);
                    newUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(newUser);
                    return jwtTokenGenerator.GetJwtToken(newUser);
                }
                else
                {
                    //add user external login and sign him in
                    await userManager.AddLoginAsync(user, info);
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    return jwtTokenGenerator.GetJwtToken(user);

                }
                
            }

            
        }
    }

}
