using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class HomeController : Controller
    {
        UnimacsContext db = new UnimacsContext();

        public ActionResult Index()
        {
            //String pcName = User.Identity.Name;
            String pcName = "unimacs3";
            var view = from p in db.ScreenSettings
                       where p.Screen.computer_name == pcName
                       select p.Page;
            String pageName = view.First().page_name;

            return RedirectToAction("Index", pageName);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}