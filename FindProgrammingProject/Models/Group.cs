namespace FindProgrammingProject.Models
{
    public interface IGroup
    {
        List<string> AdminId { get; set; }
        string Description { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        List<string> PeopleId { get; set; }
        byte[] ProfilePhoto { get; set; }
        List<string> ProjectId { get; set; }
        Dictionary<string, string> UserRoles { get; set; }
    }

    public class Group : IGroup
    {
        private string id;
        private string name;
        private string description;
        private byte[] profilePhoto;
        private List<string> projectId;
        private List<string> adminId;
        private List<string> peopleId;
        private Dictionary<string, string> userRoles;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public byte[] ProfilePhoto { get => profilePhoto; set => profilePhoto = value; }
        public List<string> ProjectId { get => projectId; set => projectId = value; }
        public List<string> AdminId { get => adminId; set => adminId = value; }
        public List<string> PeopleId { get => peopleId; set => peopleId = value; }
        public Dictionary<string, string> UserRoles { get => userRoles; set => userRoles = value; }
    }
}
