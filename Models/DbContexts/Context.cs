using System;
using System.Collections.Generic;
using FindProgrammingProject.Models.NonObjectModels;
using FindProgrammingProject.Models.ObjectModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FindProgrammingProject.Models.DbContexts
{
    public partial class Context : IdentityDbContext<User>
    {
        public Context()
        {
        }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<CommentUpvote> CommentUpvotes { get; set; } = null!;
        public virtual DbSet<Group> Groups { get; set; } = null!;
        public virtual DbSet<GroupProject> GroupProjects { get; set; } = null!;
        public virtual DbSet<Invite> Invites { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectUpvote> ProjectUpvotes { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;
        public virtual DbSet<UserProject> UserProjects { get; set; } = null!;
		public virtual DbSet<UserPlan> UserPlans { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var PeopleNeededToLanguageConverter = new ValueConverter<List<PeopleNeededToLanguage>, string>(v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<PeopleNeededToLanguage>>(v));


            var converter = new ValueConverter<List<string>, string>(v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v));

            modelBuilder.Entity<User>().Property(x => x.ProgrammingLanguages).HasConversion(converter);
            modelBuilder.Entity<User>().Property(x => x.ProjectTypes).HasConversion(converter);
            modelBuilder.Entity<Project>().Property(x => x.PeopleNeededToLanguage).HasConversion(PeopleNeededToLanguageConverter);


            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.ProjectId, "IX_Comments_ProjectId");

                entity.HasIndex(e => e.UserId, "IX_Comments_UserId");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProjectId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<CommentUpvote>(entity =>
            {
                entity.HasKey(e => new { e.CommentId, e.UserId });

                entity.HasIndex(e => e.UserId, "IX_CommentUpvotes_UserId");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.CommentUpvotes)
                    .HasForeignKey(d => d.CommentId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CommentUpvotes)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasIndex(e => e.CreatorId, "IX_Groups_CreatorId");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.CreatorId);
            });

            modelBuilder.Entity<GroupProject>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.ProjectId });

                entity.ToTable("GroupProject");

                entity.HasIndex(e => e.ProjectId, "IX_GroupProject_ProjectId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupProjects)
                    .HasForeignKey(d => d.GroupId);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.GroupProjects)
                    .HasForeignKey(d => d.ProjectId);
            });

            modelBuilder.Entity<Invite>(entity =>
            {
                entity.HasIndex(e => e.FromId, "IX_Invites_FromId");

                entity.HasIndex(e => e.ProjectId, "IX_Invites_ProjectId");

                entity.Property(e => e.ToEntityName).HasDefaultValueSql("(N'')");


                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.InviteFromUsers)
                    .HasForeignKey(d => d.FromId);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Invites)
                    .HasForeignKey(d => d.ProjectId);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.CreatorId, "IX_Projects_CreatorId");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.CreatorId);
            });

            modelBuilder.Entity<ProjectUpvote>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProjectId });

                entity.HasIndex(e => e.ProjectId, "IX_ProjectUpvotes_ProjectId");

                entity.Property(e => e.ProjectCreatorId).HasDefaultValueSql("(N'')");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectUpvotes)
                    .HasForeignKey(d => d.ProjectId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProjectUpvotes)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.GroupId });

                entity.ToTable("UserGroup");

                entity.HasIndex(e => e.GroupId, "IX_UserGroup_GroupId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.GroupId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserProject>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProjectId });

                entity.ToTable("UserProject");

                entity.HasIndex(e => e.ProjectId, "IX_UserProject_ProjectId");

                entity.Property(e => e.IsAdmin)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.UserProjects)
                    .HasForeignKey(d => d.ProjectId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProjects)
                    .HasForeignKey(d => d.UserId);
            });

			modelBuilder.Entity<UserPlan>(entity =>
			{
				entity.ToTable("UserPlan");

				entity.HasIndex(e => e.UserId, "IX_UserPlan_UserId")
					.IsUnique();

				entity.HasOne<User>()
					.WithOne()
					.HasForeignKey<UserPlan>(d => d.UserId);
			});

			OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
