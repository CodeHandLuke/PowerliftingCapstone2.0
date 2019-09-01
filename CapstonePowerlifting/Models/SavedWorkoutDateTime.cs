using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CapstonePowerlifting.Models
{
	public class SavedWorkoutDateTime
	{
		[Key]
		public int SavedWorkoutDateTimeId { get; set; }
		[Display(Name = "Workout Completion")]
		public DateTime Date { get; set; }
		[Display(Name = "Squat Reps")]
		public string CompletedSquatReps { get; set; }
		[Display(Name = "Squat Weight")]
		public string CompletedSquatWeight { get; set; }
		[Display(Name = "Bench Reps")]
		public string CompletedBenchReps { get; set; }
		[Display(Name = "Bench Weight")]
		public string CompletedBenchWeight { get; set; }
		[Display(Name = "Deadlift Reps")]
		public string CompletedDeadliftReps { get; set; }
		[Display(Name = "Deadlift Weight")]
		public string CompletedDeadliftWeight { get; set; }
		public virtual int? ActualSquatReps { get; set; }
		public virtual int? ExpectedSquatReps { get; set; }
		public virtual int? ActualBenchReps { get; set; }
		public virtual int? ExpectedBenchReps { get; set; }
		public virtual int? ActualDeadliftReps { get; set; }
		public virtual int? ExpectedDeadliftReps { get; set; }
		public virtual double? ActualSquatWeight { get; set; }
		public virtual double? ExpectedSquatWeight { get; set; }
		public virtual double? ActualBenchWeight { get; set; }
		public virtual double? ExpectedBenchWeight { get; set; }
		public virtual double? ActualDeadliftWeight { get; set; }
		public virtual double? ExpectedDeadliftWeight { get; set; }
		[ForeignKey(nameof(User))]
		public int UserId { get; set; }
		public UserProfile User { get; set; }
	}
}