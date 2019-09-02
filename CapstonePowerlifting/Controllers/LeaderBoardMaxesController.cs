using CapstonePowerlifting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapstonePowerlifting.Controllers
{
    public class LeaderBoardMaxesController : Controller
    {

		private ApplicationDbContext db = new ApplicationDbContext();
		// GET: LeaderBoardMaxes
		public ActionResult Index()
        {
			var leaderboardMaxes = db.LeaderboardMaxes.ToList();
			return View(leaderboardMaxes.OrderByDescending(o => o.Wilks));
		}

        // GET: LeaderBoardMaxes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LeaderBoardMaxes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaderBoardMaxes/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaderBoardMaxes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaderBoardMaxes/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaderBoardMaxes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaderBoardMaxes/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
