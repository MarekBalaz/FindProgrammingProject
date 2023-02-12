using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class UserGroup
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public bool IsAdmin { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
