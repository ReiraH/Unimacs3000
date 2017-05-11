using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Unimacs_3000.Controllers
{
    public class SensordataController : Controller
    {
        // GET: Sensordata
        public ActionResult Index()
        {
            return View();
        }

        // GET: Sensordata/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Sensordata/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sensordata/Create
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

        // GET: Sensordata/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Sensordata/Edit/5
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

        // GET: Sensordata/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sensordata/Delete/5
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
