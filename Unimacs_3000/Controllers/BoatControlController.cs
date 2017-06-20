using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class BoatControlController : Controller
    {
        UnimacsContext db = new UnimacsContext();
        // GET: Simulatie
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult checkBoatData()
        {
            BoatMotion boatMotion = db.BoatMotions.OrderByDescending(sd => sd.Timestamp).First();

            //return Json(new { result = "DoNothing" });
            return Json(
                new {
                        result = "UpdateData",
                        leftEngine = Math.Round(boatMotion.LeftEngineValue,2),
                        rightEngine = Math.Round(boatMotion.RightEngineValue,2),
                        rudder = Math.Round(boatMotion.RudderValue,2),
                    }
                );
        }
    }
}
