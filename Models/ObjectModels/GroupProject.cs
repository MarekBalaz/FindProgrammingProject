using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class GroupProject
    {
        public string GroupId { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
        public string? Id { get; set; }
        public bool IsAdmin { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
