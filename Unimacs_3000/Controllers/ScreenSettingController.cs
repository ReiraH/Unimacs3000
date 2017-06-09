using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class ScreenSettingController : Controller
    {
        UnimacsContext db = new UnimacsContext();
        // GET: View
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult onChange(String currentController)
        {
            String pcName = User.Identity.Name;
            pcName = pcName.Split('\\')[1];
            if(pcName.Equals("esatk"))
            {
                pcName = "Unimacs001";
            }
            var view = from p in db.ScreenSettings
                       where p.Screen.computer_name == pcName
                       select p.Page;
            String pageName = view.First().page_name;
            if (currentController.Contains(pageName))
            {
                return Json(new { result = "DoNothing" });
            }
            return Json(new { result = "Redirect", url = Url.Action("Index", pageName) });
        }
    }
}