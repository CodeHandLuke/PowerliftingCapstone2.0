﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using CapstonePowerlifting.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace CapstonePowerlifting.Controllers
{
	public class LiftsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Lifts
		public ActionResult Index()
		{
			var appUserId = User.Identity.GetUserId();
			if (appUserId == null)
			{
				return RedirectToAction("Login", "Account");
			}
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			if (currentUser == null)
			{
				return RedirectToAction("Create", "UserProfiles");
			}
			var oneRepMaxCount = db.OneRepMaxes.Where(m => m.UserId == currentUser.UserId).Count();
			if (oneRepMaxCount < 1)
			{
				MessageBox.Show("Please input your one-rep maxes to initialize your workout!");
				return RedirectToAction("Index", "OneRepMaxes");
			}
			var lifts = db.Lifts.Where(o => o.WorkoutId == currentUser.WorkoutOfDay && o.UserId == currentUser.UserId);
			return View(lifts.ToList());
		}

		public ActionResult InitializeWorkout()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			SeedLiftsTable();
			DetermineLiftWeights();
			DetermineExpectedProgramTotals(currentUser.UserId);
			return RedirectToAction("Index");
		}

		// GET: Lifts/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Lift lift = db.Lifts.Find(id);
			if (lift == null)
			{
				return HttpNotFound();
			}
			return View(lift);
		}

		// GET: Lifts/Create
		public ActionResult Create()
		{
			ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName");
			return View();
		}

		// POST: Lifts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ProgramId,SetOrder,WorkoutId,Exercise,OneRMPercentage,Reps,Weight,Completed,Notes,NoteText,UserId")] Lift lift)
		{
			if (ModelState.IsValid)
			{
				var appUserId = User.Identity.GetUserId();
				var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
				lift.UserId = currentUser.UserId;
				db.Lifts.Add(lift);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", lift.UserId);
			return View(lift);
		}

		// GET: Lifts/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Lift lift = db.Lifts.Find(id);
			if (lift == null)
			{
				return HttpNotFound();
			}
			ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", lift.UserId);
			return View(lift);
		}

		// POST: Lifts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ProgramId,SetOrder,WorkoutId,Exercise,OneRMPercentage,Reps,Weight,Completed,Notes,NoteText,UserId")] Lift lift)
		{
			if (ModelState.IsValid)
			{
				if (lift.NoteText == null)
				{
					lift.Notes = false;
					db.Entry(lift).State = EntityState.Modified;
					db.SaveChanges();
				}
				else
				{
					lift.Notes = true;
					db.Entry(lift).State = EntityState.Modified;
					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "FirstName", lift.UserId);
			return View(lift);
		}

		// GET: Lifts/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Lift lift = db.Lifts.Find(id);
			if (lift == null)
			{
				return HttpNotFound();
			}
			return View(lift);
		}

		// POST: Lifts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Lift lift = db.Lifts.Find(id);
			db.Lifts.Remove(lift);
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

		//This method will be used to calculate the how heavy each lift has to be using the lifter's one rep max
		public void DetermineLiftWeights()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var oneRepMaxCount = db.OneRepMaxes.Where(m => m.UserId == currentUser.UserId).Count();
			var oneRepMaxList = db.OneRepMaxes.Where(o => o.UserId == currentUser.UserId).ToList();
			var oneRepMax = oneRepMaxList.LastOrDefault();
			var squatM = oneRepMax.Squat;
			var benchMax = oneRepMax.Bench;
			var deadMax = oneRepMax.Deadlift;
			var foundLifts = db.Lifts.ToList();
			foreach (var item in foundLifts)
			{
				var oneRepMaxMultiplier = item.OneRMPercentage * .01;
				if (item.Exercise == "Squat")
				{
					double? calculatedWeight = squatM * oneRepMaxMultiplier;
					item.Weight = Math.Round(Convert.ToDouble(calculatedWeight), 2);
					db.SaveChanges();
				}

				else if (item.Exercise == "Benchpress")
				{
					double? calculatedWeight = benchMax * oneRepMaxMultiplier;
					item.Weight = Math.Round(Convert.ToDouble(calculatedWeight), 2);
					db.SaveChanges();
				}

				else if (item.Exercise == "Deadlift" || item.Exercise == "Deadlift^Knee" || item.Exercise == "Def Deadlift" || item.Exercise == "Rackpull")
				{
					double? calculatedWeight = deadMax * oneRepMaxMultiplier;
					item.Weight = Math.Round(Convert.ToDouble(calculatedWeight), 2);
					db.SaveChanges();
				}
			}
		}

		public ActionResult CompleteWorkout()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var userId = currentUser.UserId;
			var completedSetCount = db.Lifts.Where(o => o.UserId == userId && o.WorkoutId == currentUser.WorkoutOfDay && o.Completed == true).Count();
			var liftsCount = db.Lifts.Where(l => l.UserId == userId).Count();
			if (liftsCount > 1)
			{
				if (completedSetCount > 1)
				{
					if (currentUser.WorkoutOfDay < 4)
					{
						SaveWorkout(currentUser.UserId, currentUser.WorkoutOfDay);
						SaveProgramTotals(currentUser.UserId, currentUser.WorkoutOfDay);
						currentUser.WorkoutOfDay++;
						db.SaveChanges();
						return RedirectToAction("Index");
					}

					else
					{
						SaveWorkout(currentUser.UserId, currentUser.WorkoutOfDay);
						SaveProgramTotals(currentUser.UserId, currentUser.WorkoutOfDay);
						currentUser.WorkoutOfDay = 1;
						db.SaveChanges();
						return RedirectToAction("SavedCompletedWorkout", "SavedWorkoutDateTimes");
					}
				}
				else if (completedSetCount < 1)
				{
					MessageBox.Show("You haven't completed enough sets to complete this workout!");
					return RedirectToAction("Index");
				}
			}
			else if (liftsCount < 1)
			{
				return RedirectToAction("Create", "OneRepMaxes");
			}

			return RedirectToAction("Index");
		}

		public void SaveProgramTotals(int userId, int? WorkoutSerial)
		{
			var foundLifts = db.Lifts.Where(l => l.UserId == userId && l.WorkoutId == WorkoutSerial && l.Completed == true).ToList();
			foreach (var item in foundLifts)
			{
				if (item.Exercise == "Squat")
				{
					var squatTotalsCount = db.ActualProgramTotals.Where(s => s.Exercise == "Squat" && s.UserId == userId).Count();
					if (squatTotalsCount < 1)
					{
						ActualProgramTotal programTotals = new ActualProgramTotal();
						programTotals.Exercise = "Squat";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ActualProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Squat").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Squat").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
				else if (item.Exercise == "Benchpress")
				{
					var benchTotalsCount = db.ActualProgramTotals.Where(s => s.Exercise == "Benchpress" && s.UserId == userId).Count();
					if (benchTotalsCount < 1)
					{
						ActualProgramTotal programTotals = new ActualProgramTotal();
						programTotals.Exercise = "Benchpress";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ActualProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Benchpress").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Benchpress").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
				else if (item.Exercise == "Deadlift")
				{
					var deadTotalsCount = db.ActualProgramTotals.Where(s => s.Exercise == "Deadlift" && s.UserId == userId).Count();
					if (deadTotalsCount < 1)
					{
						ActualProgramTotal programTotals = new ActualProgramTotal();
						programTotals.Exercise = "Deadlift";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ActualProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Deadlift").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ActualProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Deadlift").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
			}
			db.SaveChanges();
		}

		public void SaveWorkout(int userId, int? WorkoutSerial)
		{
			var foundLifts = db.Lifts.Where(l => l.UserId == userId && l.WorkoutId == WorkoutSerial).ToList();
			foreach (var item in foundLifts)
			{
				SavedWorkout newSavedWorkout = new SavedWorkout();
				newSavedWorkout.Date = DateTime.Now;
				newSavedWorkout.Exercise = item.Exercise;
				newSavedWorkout.OneRMPercentage = item.OneRMPercentage;
				newSavedWorkout.Reps = item.Reps;
				newSavedWorkout.Weight = item.Weight;
				newSavedWorkout.WorkoutId = item.WorkoutId;
				newSavedWorkout.NoteText = item.NoteText;
				newSavedWorkout.UserId = userId;
				db.SavedWorkouts.Add(newSavedWorkout);
			}
			db.SaveChanges();
		}

		public ActionResult CompleteAllReps()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var userId = currentUser.UserId;
			var lifts = db.Lifts.Where(o => o.WorkoutId == currentUser.WorkoutOfDay && o.UserId == userId).ToList();
			if (lifts.Count > 1)
			{
				foreach (var item in lifts)
				{
					item.Completed = true;
				}
				db.SaveChanges();
			}

			return RedirectToAction("Index");
		}

		public void DetermineExpectedProgramTotals(int userId)
		{
			var allLifts = db.Lifts.Where(l => l.UserId == userId).ToList();
			ExpectedProgramTotal expectedProgramTotal = new ExpectedProgramTotal();
			foreach (var item in allLifts)
			{
				if (item.Exercise == "Squat")
				{
					var squatTotalsCount = db.ExpectedProgramTotals.Where(s => s.Exercise == "Squat" && s.UserId == userId).Count();
					if (squatTotalsCount < 1)
					{
						ExpectedProgramTotal programTotals = new ExpectedProgramTotal();
						programTotals.Exercise = "Squat";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ExpectedProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Squat").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Squat").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
				else if (item.Exercise == "Benchpress")
				{
					var benchTotalsCount = db.ExpectedProgramTotals.Where(s => s.Exercise == "Benchpress" && s.UserId == userId).Count();
					if (benchTotalsCount < 1)
					{
						ExpectedProgramTotal programTotals = new ExpectedProgramTotal();
						programTotals.Exercise = "Benchpress";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ExpectedProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Benchpress").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Benchpress").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
				else if (item.Exercise == "Deadlift")
				{
					var deadTotalsCount = db.ExpectedProgramTotals.Where(s => s.Exercise == "Deadlift" && s.UserId == userId).Count();
					if (deadTotalsCount < 1)
					{
						ExpectedProgramTotal programTotals = new ExpectedProgramTotal();
						programTotals.Exercise = "Deadlift";
						programTotals.Reps = 0;
						programTotals.Weight = 0;
						programTotals.UserId = userId;
						db.ExpectedProgramTotals.Add(programTotals);
						db.SaveChanges();
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Deadlift").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
					else
					{
						var foundProgramTotals = db.ExpectedProgramTotals.Where(f => f.UserId == userId && f.Exercise == "Deadlift").FirstOrDefault();
						foundProgramTotals.Reps += item.Reps;
						foundProgramTotals.Weight += item.Weight * item.Reps;
						db.SaveChanges();
					}
				}
			}
			db.SaveChanges();
		}

		public int? CalculatePercentage(int? part, int? whole)
		{
			int? answer;
			double fraction = (double)part / (double)whole;
			answer = Convert.ToInt32(fraction * 100);
			return answer;
		}

		int setOrder = 1;

		public void SeedLiftsTable()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var liftsCount = db.Lifts.Where(l => l.UserId == currentUser.UserId).Count();
			if (liftsCount < 45)
			{
				SeedWorkoutTableDayOne();
				SeedWorkoutTableDayTwo();
				SeedWorkoutTableDayThree();
				SeedWorkoutTableDayFour();
			}
		}
		public void SeedWorkoutTableDayOne()
		{
			SeedWorkoutTableSquat();
			SeedWorkoutTableBench();
			SeedWorkoutTableSquatSetTwo();
			setOrder = 1;
		}
		public void SeedWorkoutTableSquat()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var squatSets = 5;
			for (int i = 0; i < squatSets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 1;
				newLift.Exercise = "Squat";
				newLift.OneRMPercentage = 50;
				newLift.Reps = 5;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableBench()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var benchSets = 5;
			for (int i = 0; i < benchSets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 1;
				newLift.Exercise = "Benchpress";
				newLift.OneRMPercentage = 50;
				newLift.Reps = 5;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableSquatSetTwo()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var squatSets = 5;
			for (int i = 0; i < squatSets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 1;
				newLift.Exercise = "Squat";
				newLift.OneRMPercentage = 70;
				newLift.Reps = 2;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableDayTwo()
		{
			SeedWorkoutTableDeadlift();
			SeedWorkoutTableBenchTwo();
			SeedWorkoutTableDeadliftTwo();
			setOrder = 1;
		}

		public void SeedWorkoutTableDeadlift()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var deadSets = 5;
			for (int i = 0; i < deadSets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 2;
				newLift.Exercise = "Deadlift";
				newLift.OneRMPercentage = 50;
				newLift.Reps = 5;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableBenchTwo()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 2;
				newLift.Exercise = "Benchpress";
				newLift.OneRMPercentage = 50;
				newLift.Reps = 6;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableDeadliftTwo()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 4;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 2;
				newLift.Exercise = "Deadlift^Knee";
				newLift.OneRMPercentage = 70;
				newLift.Reps = 4;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableDayThree()
		{
			SeedWorkoutTableBenchThree();
			SeedWorkoutTableSquatThree();
			SeedWorkoutTableBenchThreeTwo();
			setOrder = 1;
		}

		public void SeedWorkoutTableBenchThree()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 3;
				newLift.Exercise = "Benchpress";
				newLift.OneRMPercentage = 80;
				newLift.Reps = 3;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableSquatThree()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 3;
				newLift.Exercise = "Squat";
				newLift.OneRMPercentage = 80;
				newLift.Reps = 3;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableBenchThreeTwo()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 3;
				newLift.Exercise = "Benchpress";
				newLift.OneRMPercentage = 75;
				newLift.Reps = 3;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableDayFour()
		{
			SeedWorkoutTableDeadliftFour();
			SeedWorkoutTableRackpull();
			setOrder = 1;
		}

		public void SeedWorkoutTableDeadliftFour()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 4;
				newLift.Exercise = "Def Deadlift";
				newLift.OneRMPercentage = 60;
				newLift.Reps = 2;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		public void SeedWorkoutTableRackpull()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var sets = 5;
			for (int i = 0; i < sets; i++)
			{
				Lift newLift = new Lift();
				newLift.SetOrder = setOrder;
				newLift.WorkoutId = 4;
				newLift.Exercise = "Rackpull";
				newLift.OneRMPercentage = 75;
				newLift.Reps = 4;
				newLift.Completed = false;
				newLift.UserId = currentUser.UserId;
				db.Lifts.Add(newLift);
				db.SaveChanges();
				setOrder++;
			}
		}

		List<int> workoutIds = new List<int>();

		public void CreateListofWorkoutIds()
		{
			if (workoutIds.Count == 0)
			{
				workoutIds.Add(1);
				workoutIds.Add(2);
				workoutIds.Add(3);
				workoutIds.Add(4);
			}
		}

		//public ActionResult FilterWorkoutDays(int workoutDay)
		//{
		//	CreateListofWorkoutIds();
		//	ViewBag.Workouts = workoutIds;
		//	var userId = ReturnCurrentUserId();
		//	var currentUser = db.UserProfiles.Where(u => u.UserId == userId).FirstOrDefault();
		//	currentUser.WorkoutOfDay = workoutDay;
		//	var foundSavedWorkouts = db.SavedWorkouts.Where(s => s.UserId == userId).ToList();
		//	foreach (var item in foundSavedWorkouts)
		//	{
		//		if (item.SavedWorkoutDateId < 1 && item.WorkoutId >= workoutDay)
		//		{
		//			db.SavedWorkouts.Remove(item);
		//		}
		//	}
		//}

		public int ReturnCurrentUserId()
		{
			var appUserId = User.Identity.GetUserId();
			var currentUser = db.UserProfiles.Where(u => u.ApplicationId == appUserId).FirstOrDefault();
			var id = currentUser.UserId;
			return id;
		}
	}
}
