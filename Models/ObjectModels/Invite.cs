using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class Invite
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string? FromId { get; set; }
        public string ToId { get; set; } = null!;
        public DateTime Date { get; set; }
        public string State { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string ToUserName { get; set; } = null!;
        public string ToUserType { get; set; } = null!;
        public string? ProjectId { get; set; }
        public string? GroupId { get; set; }
        public string ToEntityName { get; set; } = null!;
        public virtual User? FromUser { get; set; }
        public virtual Project? Project { get; set; }
    }
}
