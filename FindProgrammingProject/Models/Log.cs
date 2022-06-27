namespace FindProgrammingProject.Models
{
    public interface ILog
    {
        DateTime Date { get; set; }
        string Id { get; set; }
        string Level { get; set; }
        string Message { get; set; }
        int StatusCode { get; set; }
    }

    public class Log : ILog
    {
        private string id;
        private int statusCode;
        private string message;
        private string level;
        private DateTime date;

        public string Id { get => id; set => id = value; }
        public int StatusCode { get => statusCode; set => statusCode = value; }
        public string Message { get => message; set => message = value; }
        public string Level { get => level; set => level = value; }
        public DateTime Date { get => date; set => date = value; }
    }
}
