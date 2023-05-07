using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class Group
    {
        public Group()
        {
            GroupProjects = new HashSet<GroupProject>();
            UserGroups = new HashSet<UserGroup>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public byte[]? ProfilePhoto { get; set; }
        public string PictureFormat { get; set; }
        public string? CreatorId { get; set; }
        public virtual User? Creator { get; set; }
        public virtual ICollection<GroupProject> GroupProjects { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}
