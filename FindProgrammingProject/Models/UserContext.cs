using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace FindProgrammingProject.Models
{
    public class UserContext : IdentityDbContext<User>
    {

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
    }
}
