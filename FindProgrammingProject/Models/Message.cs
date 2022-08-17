using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FindProgrammingProject.Models
{
    public interface IMessage
    {
        DateTime Date { get; set; }
        string Id { get; set; }
        string MessageContent { get; set; }
        string PersonFromId { get; set; }
        string PersonToId { get; set; }
        bool UnRead { get; set; }
    }

    public class Message : IMessage
    {
        [Key]
        private string id;
        private string personFromId;
        private string personToId;
        private string messageContent;
        private DateTime date;
        private bool unread;

        public string Id { get => id; set => id = value; }
        public string PersonFromId { get => personFromId; set => personFromId = value; }
        public string PersonToId { get => personToId; set => personToId = value; }
        public string MessageContent { get => messageContent; set => messageContent = value; }
        public DateTime Date { get => date; set => date = value; }
        public bool UnRead { get => unread; set => unread = value; }
    }
    
}
