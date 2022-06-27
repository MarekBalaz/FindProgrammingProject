using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;

namespace FindProgrammingProject.FunctionalClasses
{
    public class SignClass : ISignClass
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ICodeGenerator codeGenerator;
        public SignClass(UserManager<User> _userManager, SignInManager<User> _signInManager, ICodeGenerator _codeGenerator)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            codeGenerator = _codeGenerator;

        }
        public async Task<int> SignIn(string Email, string Password)
        {   
            var user = await userManager.FindByEmailAsync(Email);
            if(user == null)
            {
                return 3;
            }
            var result = await signInManager.PasswordSignInAsync(Email,Password,true,false);
            if(result.Succeeded)
            {
                return 1;
            }
            else if(result.IsLockedOut)
            {
                return 2;
            }
            else
            {
                if(user.EmailConfirmed == false)
                {
                    return 4;
                }
                return 5;
            }
        }
        public async Task<int> SignOut()
        {
            await signInManager.SignOutAsync();
            return 1;
        }
        public async Task<int> SignUp(string Email,string Nickname, string Password, string PasswordConfirmation)
        {
            if(Password == PasswordConfirmation)
            {
                var result = await userManager.FindByEmailAsync(Email);
                if(result == null)
                {
                    //Here we will generate email confirmation code and send it to email
                    return 1;
                }
                else
                {
                    //Here we will return email already exist
                    return 2;
                }
            }
            else
            {
                return 3;
            }
               
        }
    }
    public interface ISignClass
    {

    }
}
