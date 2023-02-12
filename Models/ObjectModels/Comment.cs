using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class Comment
    {
        public Comment()
        {
            CommentUpvotes = new HashSet<CommentUpvote>();
        }

        public string Id { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? UserId { get; set; }
        public DateTime DateOfCreation { get; set; }
        public string ProjectId { get; set; } = null!;
        public string? ReplyToCommentId { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual User? User { get; set; }
        public virtual ICollection<CommentUpvote> CommentUpvotes { get; set; }
    }
}
