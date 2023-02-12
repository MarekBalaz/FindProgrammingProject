using Microsoft.AspNetCore.Identity;

namespace FindProgrammingProject.Models.ObjectModels
{
    public class User : IdentityUser
    {
        public byte[]? ProfilePicture { get; set; }

        public string Description { get; set; } = null!;

        public List<string> ProgrammingLanguages { get; set; } = null!;

        public List<string> ProjectTypes { get; set; } = null!;

        public List<string> BannedUsers { get; set; } = null!;

        public string WebSocketId { get; set; } = null!;

        public virtual ICollection<CommentUpvote> CommentUpvotes { get; } = new List<CommentUpvote>();

        public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

        public virtual ICollection<Group> Groups { get; } = new List<Group>();

        public virtual ICollection<Invite> InviteFromUsers { get; } = new List<Invite>();

        public virtual ICollection<Invite> InviteFroms { get; } = new List<Invite>();

        public virtual ICollection<ProjectUpvote> ProjectUpvotes { get; } = new List<ProjectUpvote>();

        public virtual ICollection<Project> Projects { get; } = new List<Project>();

        public virtual ICollection<UserGroup> UserGroups { get; } = new List<UserGroup>();

        public virtual ICollection<UserProject> UserProjects { get; } = new List<UserProject>();
    }
}
