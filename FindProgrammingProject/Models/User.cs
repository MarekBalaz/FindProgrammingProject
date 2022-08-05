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
        List<string> ProgrammingLanguages { get; set; }
        List<string> ProjectTypes { get; set; }
        List<string> BannedUsers { get; set; }
        string WebSocketId { get; set; }
        List<string> Contacts { get; set; }
    }

    public class User : IdentityUser, IUser
    {
        
        private byte[] profilePicture;
        private string description;
        private List<string> projectId;
        private List<string> groupId;
        private List<string> messageId;
        private List<string> commentId;
        private List<string> programmingLanguages;
        private List<string> projectTypes;
        private List<string> bannedusers;
        private string websocketid;
        private List<string> contacts;
        

        public byte[] ProfilePicture { get => profilePicture; set => profilePicture = value; }
        public string Description { get => description; set => description = value; }
        public List<string> ProjectId { get => projectId; set => projectId = value; }
        public List<string> GroupId { get => groupId; set => groupId = value; }
        public List<string> MessageId { get => messageId; set => messageId = value; }
        public List<string> CommentId { get => commentId; set => commentId = value; }
        public List<string> ProgrammingLanguages { get => programmingLanguages; set => programmingLanguages = value; }
        public List<string> ProjectTypes { get => projectTypes; set => projectTypes = value; }
        public List<string> BannedUsers { get => bannedusers; set => bannedusers = value; }
        public string WebSocketId { get => websocketid; set => websocketid = value; }
        public List<string> Contacts { get => contacts; set => contacts = value; }
    }
}
