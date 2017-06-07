using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class SensordataController : Controller
    {
        //initialiseer onze database
        UnimacsContext db = new UnimacsContext();
        // GET: Sensordata
        public ActionResult Index()
        {

            //laat index pagina zien
            return View(db.SensorDatas.ToList());
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
    }
}
