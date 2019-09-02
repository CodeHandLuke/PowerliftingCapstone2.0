using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CapstonePowerlifting.Models
{
	public class LeaderboardMax
	{
		[Key]
		public int LeaderboardMaxId { get; set; }
		public string UserName { get; set; }
		public int Age { get; set; }
		public double Weight { get; set; }
		[Display(Name = "Squat(kg)")]
		public double Squat { get; set; }
		[Display(Name = "Bench(kg)")]
		public double Bench { get; set; }
		[Display(Name = "Deadlift(kg)")]
		public double Deadlift { get; set; }
		[Display(Name = "Total(kg)")]
		public double Total { get; set; }
		public double Wilks { get; set; }
		public int UserId { get; set; }
	}
}