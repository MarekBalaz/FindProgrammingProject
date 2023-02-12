using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class ProjectUpvote
    {
        public string? Id { get; set; }
        public string ProjectId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string ProjectCreatorId { get; set; } = null!;
        public int Upvote { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
