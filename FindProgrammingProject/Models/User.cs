using Microsoft.AspNetCore.Identity;

namespace FindProgrammingProject.Models
{
    public interface IUser
    {
        List<string> CommentId { get; set; }
        string Description { get; set; }
        List<string> GroupId { get; set; }
        List<string> MessageId { get; set; }
        byte[] ProfilePicture { get; set; }
        List<string> ProjectId { get; set; }
    }

    public class User : IdentityUser, IUser
    {
        
        private byte[] profilePicture;
        private string description;
        private List<string> projectId;
        private List<string> groupId;
        private List<string> messageId;
        private List<string> commentId;

        public byte[] ProfilePicture { get => profilePicture; set => profilePicture = value; }
        public string Description { get => description; set => description = value; }
        public List<string> ProjectId { get => projectId; set => projectId = value; }
        public List<string> GroupId { get => groupId; set => groupId = value; }
        public List<string> MessageId { get => messageId; set => messageId = value; }
        public List<string> CommentId { get => commentId; set => commentId = value; }
    }
}
