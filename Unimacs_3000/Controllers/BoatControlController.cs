using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Unimacs_3000.Controllers
{
    public class BoatControlController : Controller
    {
        // GET: Simulatie
        public ActionResult Index()
        {
            return View();
        }

        // GET: Simulatie/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Simulatie/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Simulatie/Create
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

        // GET: Simulatie/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Simulatie/Edit/5
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

        // GET: Simulatie/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Simulatie/Delete/5
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
