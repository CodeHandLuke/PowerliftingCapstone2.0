using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CapstonePowerlifting.Models;
using Microsoft.AspNet.Identity;

namespace CapstonePowerlifting.Controllers
{
    public class SavedWorkoutDateTimesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SavedWorkoutDateTimes
        public ActionResult Index()
        {
			var userId = ReturnCurrentUserId();
			var savedWorkoutDateTimes = db.SavedWorkoutDateTimes.Where(w => w.UserId == userId).ToList();
            return View(savedWorkoutDateTimes.ToList());
        }

		public ActionResult ViewHistoricalWorkout(int? id)
		{
			var savedWorkouts = db.SavedWorkouts.Where(s => s.SavedWorkoutDateId == id).ToList();
			return View(savedWorkouts.ToList());
		}

		// GET: SavedWorkoutDateTimes/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedWorkoutDateTime savedWorkoutDateTime = db.SavedWorkoutDateTimes.Find(id);
            if (savedWorkoutDateTime == null)
            {
                return HttpNotFound();
            }
			var foundSavedWorkouts = db.SavedWorkouts.Where(s => s.SavedWorkoutDateId == id).ToList();
            return View(foundSavedWorkouts.ToList());
        }

        // GET: SavedWorkoutDateTimes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName");
            return View();
        }

        // POST: SavedWorkoutDateTimes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavedWorkoutDateTimeId,Date,CompletedSquatReps,CompletedSquatWeight,CompletedBenchReps,CompletedBenchWeight,CompletedDeadliftReps,CompletedDeadliftWeight,ActualSquatReps,ExpectedSquatReps,ActualBenchReps,ExpectedBenchReps,ActualDeadliftReps,ExpectedDeadliftReps,ActualSquatWeight,ExpectedSquatWeight,ActualBenchWeight,ExpectedBenchWeight,ActualDeadliftWeight,ExpectedDeadliftWeight,UserId")] SavedWorkoutDateTime savedWorkoutDateTime)
        {
            if (ModelState.IsValid)
            {
				var appUserId = User.Identity.GetUserId();
				var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
				savedWorkoutDateTime.UserId = currentUser.UserId;
				db.SavedWorkoutDateTimes.Add(savedWorkoutDateTime);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", savedWorkoutDateTime.UserId);
            return View(savedWorkoutDateTime);
        }

        // GET: SavedWorkoutDateTimes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedWorkoutDateTime savedWorkoutDateTime = db.SavedWorkoutDateTimes.Find(id);
            if (savedWorkoutDateTime == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", savedWorkoutDateTime.UserId);
            return View(savedWorkoutDateTime);
        }

        // POST: SavedWorkoutDateTimes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SavedWorkoutDateTimeId,Date,CompletedSquatReps,CompletedSquatWeight,CompletedBenchReps,CompletedBenchWeight,CompletedDeadliftReps,CompletedDeadliftWeight,ActualSquatReps,ExpectedSquatReps,ActualBenchReps,ExpectedBenchReps,ActualDeadliftReps,ExpectedDeadliftReps,ActualSquatWeight,ExpectedSquatWeight,ActualBenchWeight,ExpectedBenchWeight,ActualDeadliftWeight,ExpectedDeadliftWeight,UserId")] SavedWorkoutDateTime savedWorkoutDateTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savedWorkoutDateTime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", savedWorkoutDateTime.UserId);
            return View(savedWorkoutDateTime);
        }

        // GET: SavedWorkoutDateTimes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedWorkoutDateTime savedWorkoutDateTime = db.SavedWorkoutDateTimes.Find(id);
            if (savedWorkoutDateTime == null)
            {
                return HttpNotFound();
            }
            return View(savedWorkoutDateTime);
        }

        // POST: SavedWorkoutDateTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SavedWorkoutDateTime savedWorkoutDateTime = db.SavedWorkoutDateTimes.Find(id);
            db.SavedWorkoutDateTimes.Remove(savedWorkoutDateTime);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

		public ActionResult SavedCompletedWorkout()
		{
			CreateSavedWorkoutDateTime();
			UpdateSavedWorkouts();
			UpdateActualProgramTotals();
			DeleteOldSavedWorkouts();
			RemovePreviousLifts();
			return RedirectToAction("Index");
		}

		public void DeleteOldSavedWorkouts()
		{
			var userId = ReturnCurrentUserId();
			var foundSavedWorkouts = db.SavedWorkouts.Where(w => w.UserId == userId).ToList();
			foreach (var item in foundSavedWorkouts)
			{
				if (item.SavedWorkoutDateId == null)
				{
					db.SavedWorkouts.Remove(item);
				}
			}
			db.SaveChanges();
		}

		public void RemovePreviousLifts()
		{
			var id = ReturnCurrentUserId();
			var foundLifts = db.Lifts.Where(l => l.UserId == id).ToList();
			var liftCount = foundLifts.Count();
			if (liftCount > 0)
			{
				foreach (var item in foundLifts)
				{
					db.Lifts.Remove(item);
				}
				db.SaveChanges();
			}
		}

		DateTime tempSavedWorkoutDate;

		public void CreateSavedWorkoutDateTime()
		{
			var userId = ReturnCurrentUserId();
			var actualSquatTotals = db.ActualProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Squat").FirstOrDefault();
			var actualBenchTotals = db.ActualProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Benchpress").FirstOrDefault();
			var actualDeadliftTotals = db.ActualProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Deadlift").FirstOrDefault();
			var expectedSquatTotals = db.ExpectedProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Squat").FirstOrDefault();
			var expectedBenchTotals = db.ExpectedProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Benchpress").FirstOrDefault();
			var expectedDeadliftTotals = db.ExpectedProgramTotals.Where(r => r.UserId == userId && r.Exercise == "Deadlift").FirstOrDefault();
			var newSavedWorkoutDateTime = new SavedWorkoutDateTime();
			newSavedWorkoutDateTime.Date = DateTime.Now;
			tempSavedWorkoutDate = newSavedWorkoutDateTime.Date;
			newSavedWorkoutDateTime.ActualSquatReps = actualSquatTotals.Reps;
			var actualSquatReps = Convert.ToString(newSavedWorkoutDateTime.ActualSquatReps);
			newSavedWorkoutDateTime.ActualSquatWeight = actualSquatTotals.Weight;
			var actualSquatWeight = Convert.ToString(newSavedWorkoutDateTime.ActualSquatWeight);
			newSavedWorkoutDateTime.ActualBenchReps = actualBenchTotals.Reps;
			var actualBenchReps = Convert.ToString(newSavedWorkoutDateTime.ActualBenchReps);
			newSavedWorkoutDateTime.ActualBenchWeight = actualBenchTotals.Weight;
			var actualBenchWeight = Convert.ToString(newSavedWorkoutDateTime.ActualBenchWeight);
			newSavedWorkoutDateTime.ActualDeadliftReps = actualDeadliftTotals.Reps;
			var actualDeadliftReps = Convert.ToString(newSavedWorkoutDateTime.ActualDeadliftReps);
			newSavedWorkoutDateTime.ActualDeadliftWeight = actualDeadliftTotals.Weight;
			var actualDeadliftWeight = Convert.ToString(newSavedWorkoutDateTime.ActualDeadliftWeight);

			newSavedWorkoutDateTime.ExpectedSquatReps = expectedSquatTotals.Reps;
			var expectedSquatReps = Convert.ToString(newSavedWorkoutDateTime.ExpectedSquatReps);
			newSavedWorkoutDateTime.ExpectedSquatWeight = expectedSquatTotals.Weight;
			var expectedSquatWeight = Convert.ToString(newSavedWorkoutDateTime.ExpectedSquatWeight);
			newSavedWorkoutDateTime.ExpectedBenchReps = expectedBenchTotals.Reps;
			var expectedBenchReps = Convert.ToString(newSavedWorkoutDateTime.ExpectedBenchReps);
			newSavedWorkoutDateTime.ExpectedBenchWeight = expectedBenchTotals.Weight;
			var expectedBenchWeight = Convert.ToString(newSavedWorkoutDateTime.ExpectedBenchWeight);
			newSavedWorkoutDateTime.ExpectedDeadliftReps = expectedDeadliftTotals.Reps;
			var expectedDeadliftReps = Convert.ToString(newSavedWorkoutDateTime.ExpectedDeadliftReps);
			newSavedWorkoutDateTime.ExpectedDeadliftWeight = expectedDeadliftTotals.Weight;
			var expectedDeadliftWeight = Convert.ToString(newSavedWorkoutDateTime.ExpectedDeadliftWeight);
			newSavedWorkoutDateTime.CompletedSquatReps = $"{actualSquatReps} / {expectedSquatReps}";
			newSavedWorkoutDateTime.CompletedSquatWeight = $"{actualSquatWeight} / {expectedSquatWeight}";
			newSavedWorkoutDateTime.CompletedBenchReps = $"{actualBenchReps} / {expectedBenchReps}";
			newSavedWorkoutDateTime.CompletedBenchWeight = $"{actualBenchWeight} / {expectedBenchWeight}";
			newSavedWorkoutDateTime.CompletedDeadliftReps = $"{actualDeadliftReps} / {expectedDeadliftReps}";
			newSavedWorkoutDateTime.CompletedDeadliftWeight = $"{actualDeadliftWeight} / {expectedDeadliftWeight}";
			newSavedWorkoutDateTime.UserId = userId;

			db.SavedWorkoutDateTimes.Add(newSavedWorkoutDateTime);
			db.SaveChanges();
		}

		public void UpdateSavedWorkouts()
		{
			var userId = ReturnCurrentUserId();
			var foundSavedWorkouts = db.SavedWorkouts.Where(f => f.UserId == userId).ToList();
			//var dateTimeId = db.SavedWorkoutDateTimes.Where(f => f.UserId == userId && f.Date == tempSavedWorkoutDate).FirstOrDefault();
			var foundSavedWorkoutDateTime = db.SavedWorkoutDateTimes.Where(f => f.UserId == userId).ToList();
			var dateTimeId = foundSavedWorkoutDateTime.LastOrDefault();
			var testDate = dateTimeId.SavedWorkoutDateTimeId;
			foreach (var item in foundSavedWorkouts)
			{
				if (item.SavedWorkoutDateId == null)
				{
					item.SavedWorkoutDateId = dateTimeId.SavedWorkoutDateTimeId;
					db.SaveChanges();
				}
			}
		}

		public void UpdateActualProgramTotals()
		{
			var userId = ReturnCurrentUserId();
			var foundSavedWorkoutDateTime = db.SavedWorkoutDateTimes.Where(f => f.UserId == userId).ToList();
			var dateTimeId = foundSavedWorkoutDateTime.LastOrDefault();
			var foundActualProgramTotals = db.ActualProgramTotals.Where(a => a.UserId == userId).ToList();
			foreach (var item in foundActualProgramTotals)
			{
				if (item.SavedWorkoutDateId == 0)
				{
					item.SavedWorkoutDateId = dateTimeId.SavedWorkoutDateTimeId;
					db.SaveChanges();
				}
			}
		}

		public int ReturnCurrentUserId()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var id = currentUser.UserId;
			return id;
		}

		HistoricalWorkoutRepository workoutRepository = new HistoricalWorkoutRepository();

		public void ExportToExcel(int? id)
		{
			string Filename = "ExcelFrom" + DateTime.Now.ToString("mm_dd_yyy_hh_ss_tt") + ".xls";
			string FolderPath = HttpContext.Server.MapPath("/ExcelFiles/");
			string FilePath = Path.Combine(FolderPath, Filename);

			//Step-1: Checking: the file name exist in server, if it is found then remove from server.------------------
			if (System.IO.File.Exists(FilePath))
			{
				System.IO.File.Delete(FilePath);
			}

			//Step-2: Get Html Data & Converted to String----------------------------------------------------------------
			string HtmlResult = RenderRazorViewToString("~/Views/SavedWorkoutDateTimes/GenerateExcel.cshtml", workoutRepository.GetSavedWorkouts(id));

			//Step-4: Html Result store in Byte[] array------------------------------------------------------------------
			byte[] ExcelBytes = Encoding.ASCII.GetBytes(HtmlResult);

			//Step-5: byte[] array converted to file Stream and save in Server------------------------------------------- 
			using (Stream file = System.IO.File.OpenWrite(FilePath))
			{
				file.Write(ExcelBytes, 0, ExcelBytes.Length);
			}

			//Step-6: Download Excel file 
			Response.ContentType = "application/vnd.ms-excel";
			Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Filename));
			Response.WriteFile(FilePath);
			Response.End();
			Response.Flush();
		}

		protected string RenderRazorViewToString(string viewName, object model)
		{
			if (model != null)
			{
				ViewData.Model = model;
			}
			using (StringWriter sw = new StringWriter())
			{
				ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);
				viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
				return sw.GetStringBuilder().ToString();
			}
		}
	}
}
