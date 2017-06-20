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
        [HttpPost]
        public ActionResult checkBoatData()
        {
            Random r = new Random();
            Double value = r.NextDouble();
            //return Json(new { result = "DoNothing" });
            return Json(
                new {
                        result = "UpdateData",
                        data = value
                    }
                );
        }
    }
}
