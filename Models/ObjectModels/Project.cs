using FindProgrammingProject.Models.NonObjectModels;
using System;
using System.Collections.Generic;

namespace FindProgrammingProject.Models.ObjectModels
{
    public partial class Project
    {
        public Project()
        {
            Comments = new HashSet<Comment>();
            GroupProjects = new HashSet<GroupProject>();
            Invites = new HashSet<Invite>();
            ProjectUpvotes = new HashSet<ProjectUpvote>();
            UserProjects = new HashSet<UserProject>();
        }

        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Started { get; set; }
        public bool Group { get; set; }
        public string? CreatorId { get; set; }
        public List<PeopleNeededToLanguage> PeopleNeededToLanguage { get; set; } = null!;
        public bool Paid { get; set; }
        public int MinimalExperience { get; set; }
        public string ProjectType { get; set; } = null!;
        public DateTime DateOfCreation { get; set; }
        public string? GroupId { get; set; }

        public virtual User? Creator { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<GroupProject> GroupProjects { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
        public virtual ICollection<ProjectUpvote> ProjectUpvotes { get; set; }
        public virtual ICollection<UserProject> UserProjects { get; set; }
    }
}
