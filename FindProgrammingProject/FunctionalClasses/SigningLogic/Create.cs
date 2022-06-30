using FindProgrammingProject.Models;
using Microsoft.AspNetCore.Identity;

namespace FindProgrammingProject.FunctionalClasses.SigningLogic
{
    public class Creation : ICreation
    {
        private UserManager<User> userManager;

        public Creation(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<User> Create(string Email, string Password, string Nickname)
        {
            User user = new User { Email = Email, UserName = Nickname };
            var result = await userManager.CreateAsync(user,Password);
            if(result.Succeeded)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
    public interface ICreation
    {
        public Task<User> Create(string Email, string Password, string Nickname);
    }
}
