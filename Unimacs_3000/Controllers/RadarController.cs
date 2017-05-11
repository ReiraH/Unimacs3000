using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Unimacs_3000.Controllers
{
    public class RadarController : Controller
    {
        // GET: Radar
        public ActionResult Index()
        {
            return View();
        }

        // GET: Radar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Radar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Radar/Create
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

        // GET: Radar/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Radar/Edit/5
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

        // GET: Radar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Radar/Delete/5
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
