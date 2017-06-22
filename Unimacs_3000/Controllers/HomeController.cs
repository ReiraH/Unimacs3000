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
        // GET : /Home/Index
        //Standard Homepage of the web application
        public ActionResult Index()
        {
            return View();
        }
    }
}