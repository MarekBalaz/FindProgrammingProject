using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class CommentUpvote
    {
        public string? Id { get; set; }
        public string CommentId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public bool IsUpvote { get; set; }

        public virtual Comment Comment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
