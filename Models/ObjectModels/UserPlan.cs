using System.ComponentModel.DataAnnotations;

namespace FindProgrammingProject.Models.ObjectModels
{
	public class UserPlan
	{
		[Key]
		public string Id { get; set; }
		public string UserId { get; set; }
		public PlanType PlanType { get; set; }
		public bool IsMonthly { get; set; }
		public float Cost { get; set; }
		public DateTime ActiveFrom { get; set; }
		public DateTime ActiveTo { get; set; }
	}
	public enum PlanType
	{
		Free,
		Standard,
		Premium
	}
}
