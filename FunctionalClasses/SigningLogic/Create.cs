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
            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = Email,
                UserName = Nickname,
                Description = "",
                ProjectId = new List<string>(),
                GroupId = new List<string>(),
                MessageId = new List<string>(),
                CommentId = new List<string>(),
                ProgrammingLanguages = new List<string>(),
                ProjectTypes = new List<string>(),
                BannedUsers = new List<string>(),
                WebSocketId = "",
                Contacts = new List<string>(),
            };
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
