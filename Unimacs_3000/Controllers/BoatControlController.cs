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
        public ActionResult CheckBoatData()
        {
            //Get boatmotion based on the timestamp (newest)
            BoatMotion boatMotion = db.BoatMotions.OrderByDescending(sd => sd.Timestamp).First();
            //Round values on 2 decimals so we can visualise them using progressbar.js
            Double leftEngine = Math.Round(boatMotion.LeftEngineValue, 2);
            Double rightEngine = Math.Round(boatMotion.RightEngineValue, 2);
            Double rudder = Math.Round(boatMotion.RudderValue, 2);
            //Json object terug sturen naar de view met de boat motion data
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
