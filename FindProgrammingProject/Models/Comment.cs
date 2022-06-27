namespace FindProgrammingProject.Models
{
    public interface IComment
    {
        string CommentContent { get; set; }
        DateTime Date { get; set; }
        string Id { get; set; }
        string PersonId { get; set; }
        string ProjectId { get; set; }
    }

    public class Comment : IComment
    {
        private string id;
        private string personId;
        private string projectId;
        private string commentContent;
        private DateTime date;

        public string Id { get => id; set => id = value; }
        public string PersonId { get => personId; set => personId = value; }
        public string ProjectId { get => projectId; set => projectId = value; }
        public string CommentContent { get => commentContent; set => commentContent = value; }
        public DateTime Date { get => date; set => date = value; }
    }
}
