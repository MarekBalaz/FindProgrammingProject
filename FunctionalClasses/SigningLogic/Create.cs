using FindProgrammingProject.Models.ObjectModels;
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
            User user = new User
            {
                Email = Email,
                UserName = Nickname,
                Description = "",
                ProgrammingLanguages = new List<string>(),
                ProjectTypes = new List<string>(),
                BannedUsers = new List<string>(),
                WebSocketId = ""
            };
            var u1 = await userManager.FindByNameAsync(Nickname);
            if(u1 != null)
            {
                return new User { UserName = "Exist" };
            }
            var u2 = await userManager.FindByEmailAsync(Email);
            if(u2 != null)
            {
                return new User { Email = "Exist" };
            }

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
