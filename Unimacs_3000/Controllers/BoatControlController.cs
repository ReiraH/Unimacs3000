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
        private UnimacsContext db = new UnimacsContext();
        // GET: Simulatie
        public ActionResult Index()
        {
            return View();
        }
        // POST : /BoatControl/CheckBoatData
        [HttpGet]
        public ActionResult getBoatData()
        {
            //Get boatmotion based on the timestamp (newest)
            BoatMotion boatMotion = db.BoatMotions.OrderByDescending(sd => sd.timestamp).First();
            //Round values on 2 decimals so we can visualise them using progressbar.js
            Double leftEngine = Math.Round(boatMotion.left_engine_value, 2);
            Double rightEngine = Math.Round(boatMotion.right_engine_value, 2);
            Double rudder = Math.Round(boatMotion.rudder_value, 2);
            //return json object back to the view
            return Json
            (
                new
                {
                    leftEngine = leftEngine,
                    rightEngine = rightEngine,
                    rudder = rudder,
                }
            );
        }
    }
}
