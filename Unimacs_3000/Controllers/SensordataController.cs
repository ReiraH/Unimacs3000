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
        //initialiseer onze database
        UnimacsContext db = new UnimacsContext();
        // GET: Sensordata

        public ActionResult Index(List<SensorData> data)
        {
            //laat index pagina zien
            return View(db.SensorDatas.OrderByDescending(sd => sd.timestamp).Distinct(new DistinctSensordataComparer()).ToList());
        }
    }
}
