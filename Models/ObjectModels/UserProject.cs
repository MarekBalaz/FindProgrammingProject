using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class UserProject
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
        public bool? IsAdmin { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
