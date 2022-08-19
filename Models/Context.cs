using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FindProgrammingProject.Models
{
    public class Context : DbContext
    {

        public DbSet<Project> Projects { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var converter = new ValueConverter<List<string>, string>(v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v));
            builder.Entity<Project>().Property(x => x.CommentsId).HasConversion(converter);

            builder.Entity<Group>().Property(x => x.ProjectId).HasConversion(converter);
        }
    }
}
