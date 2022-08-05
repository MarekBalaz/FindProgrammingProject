namespace FindProgrammingProject.Models
{
    public interface IProject
    {
        List<string> CommentsId { get; set; }
        List<string> CoWorkersId { get; set; }
        string CreatorId { get; set; }
        DateTime DateOfCreation { get; set; }
        string Description { get; set; }
        bool Group { get; set; }
        string Id { get; set; }
        int MinimalExperience { get; set; }
        bool Paid { get; set; }
        List<(string, int)> PeopleNeededToLanguage { get; set; }
        string ProjectType { get; set; }
        bool Started { get; set; }
        string Title { get; set; }
        int Upvotes { get; set; }
    }

    public class Project : IProject
    {
        private string id;
        private string title;
        private string description;
        private int upvotes;
        private bool started;
        private bool group;
        private string creatorId;
        private List<string> coWorkersId;
        private List<string> commentsId;
        private List<(string, int)> peopleNeededToLanguage;
        private bool paid;
        private int minimalExperience;
        private string projectType;
        private DateTime dateOfCreation;

        public string Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public int Upvotes { get => upvotes; set => upvotes = value; }
        public bool Started { get => started; set => started = value; }
        public bool Group { get => group; set => group = value; }
        public string CreatorId { get => creatorId; set => creatorId = value; }
        public List<string> CoWorkersId { get => coWorkersId; set => coWorkersId = value; }
        public List<string> CommentsId { get => commentsId; set => commentsId = value; }
        public List<(string, int)> PeopleNeededToLanguage { get => peopleNeededToLanguage; set => peopleNeededToLanguage = value; }
        public bool Paid { get => paid; set => paid = value; }
        public int MinimalExperience { get => minimalExperience; set => minimalExperience = value; }
        public string ProjectType { get => projectType; set => projectType = value; }
        public DateTime DateOfCreation { get => dateOfCreation; set => dateOfCreation = value; }
    }
}
