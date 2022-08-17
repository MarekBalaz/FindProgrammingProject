using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Text.Json;

namespace FindProgrammingProject.Models
{
    public class UserContext : IdentityDbContext<User>
    {

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var converter = new ValueConverter<List<string>, string>(v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v));
            builder.Entity<User>().Property(x => x.ProjectId).HasConversion(converter);
            builder.Entity<User>().Property(x => x.GroupId).HasConversion(converter);
            builder.Entity<User>().Property(x => x.MessageId).HasConversion(converter);
            builder.Entity<User>().Property(x => x.CommentId).HasConversion(converter);
            builder.Entity<User>().Property(x => x.ProgrammingLanguages).HasConversion(converter);
            builder.Entity<User>().Property(x => x.ProjectTypes).HasConversion(converter);
            builder.Entity<User>().Property(x => x.BannedUsers).HasConversion(converter);
            builder.Entity<User>().Property(x => x.Contacts).HasConversion(converter);
        }
    }
}
