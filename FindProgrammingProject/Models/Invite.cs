namespace FindProgrammingProject.Models
{
    public class Invite : IInvite
    {


        private string id;
        private string title;
        private string text;
        private string fromId;
        private string toId;
        private DateTime date;
        

        public string Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Text { get => text; set => text = value; }
        public string FromId { get => fromId; set => fromId = value; }
        public string ToId { get => toId; set => toId = value; }
        public DateTime Date { get => date; set => date = value; }
    }
    public interface IInvite
    {
        string Id { get; set; }
        string Title { get; set; }
        string Text { get; set; }
        string FromId { get; set; }
        string ToId { get; set; }
        DateTime Date { get; set; }
    }
}
