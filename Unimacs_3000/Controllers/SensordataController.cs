using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Helpers;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class SensordataController : Controller
    {
        //initialise database
        private UnimacsContext db = new UnimacsContext();
        // GET: Sensordata/Index
        public ActionResult Index(List<SensorData> data)
        {
            //Get all sensor data and sort by newest
            //After this distinct the values based on name (see DitinctSensordataComparer in Helpers folder)
            //Show view with sensordata
            return View(db.SensorDatas.OrderByDescending(sd => sd.timestamp).ToList().Distinct(new DistinctSensordataComparer()).ToList());
        }
    }
}
