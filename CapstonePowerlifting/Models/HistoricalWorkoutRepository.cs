using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CapstonePowerlifting.Models
{
	public class HistoricalWorkoutRepository
	{
		ApplicationDbContext db = new ApplicationDbContext();
		public List<SavedWorkout> GetSavedWorkouts(int? id)
		{
			var savedWorkouts = db.SavedWorkouts.Where(s => s.SavedWorkoutDateId == id).ToList();
			return savedWorkouts;
		}
	}
}