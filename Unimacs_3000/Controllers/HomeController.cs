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
            return View();
        }
    }
}