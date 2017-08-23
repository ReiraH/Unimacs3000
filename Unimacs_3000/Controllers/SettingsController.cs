using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unimacs_3000.Models;

namespace Unimacs_3000.Controllers
{
    public class SettingsController : Controller
    {
        private UnimacsContext db = new UnimacsContext();

        // GET: Settings
        public ActionResult Index()
        {
            //Get all screensettings from db
            //Foreach screensettings we are going to add all possible pages in a list, so we can select and change it
            foreach (ScreenSetting ss in db.ScreenSettings.ToList())
            {
                List<SelectListItem> selectList = new List<SelectListItem>();
                foreach (Page p in db.Pages.ToList())
                {
                    selectList.Add(new SelectListItem { Text = p.page_name, Value = p.id.ToString(), Selected = (ss.page_id == p.id) });
                }
                //IF ERROR -> Add public List<SelectListItem> SelectListItems = new List<SelectListItem>(); to the Screensettings model
                //Model can be found in Models->UnimacsDataModel.edmx->UnimacsDataModel.tt->ScreenSetting.cs
                ss.SelectListItems = selectList;
            }
            return View(db.ScreenSettings.ToList());
        }
        [HttpPost]
        public ActionResult checkCurrentScreenPage(String currentController)
        {
            //Get current authenticated username
            String userName = User.Identity.Name.Split('\\')[1];
            String pageName = "";
            //TESTPURPOSES
            //Get page of the current username in the database
            var view = from p in db.ScreenSettings
                       where p.Screen.computer_name == userName
                       select p.Page;

            try
            {
                pageName = view.First().page_name;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //If the control did not change do nothing , or else sent redirect this screen to new page
            if (currentController.Contains(pageName))
            {
                return Json(new { result = "DoNothing" });
            }
            return Json(new { result = "Redirect", url = Url.Action("Index", pageName) });
        }

        // POST: Settings/Update
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        //This function is called when a setting changes in the settingsview
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                List<String> screenSettingIds = Request.Form["id"].Split(',').ToList();
                List<String> screenIds = Request.Form["screen_id"].Split(',').ToList();
                List<String> pageIds = Request.Form["page_id"].Split(',').ToList();

                for (int i = 0; i < screenSettingIds.Count(); i++)
                {
                    //Convert strings to ints
                    int id = Int32.Parse(screenSettingIds[i]);
                    int pageId = Int32.Parse(pageIds[i]);
                    int screenId = Int32.Parse(screenIds[i]);

                    //Change the page of the screen in the database
                    ScreenSetting screenSetting = db.ScreenSettings.Find(id);
                    screenSetting.Page = db.Pages.Find(pageId);
                    screenSetting.Screen = db.Screens.Find(screenId);
                    db.Entry(screenSetting).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Index", "Settings");
        }

        public ActionResult ChangePage(string newPage)
        {
            String userName = User.Identity.Name.Split('\\')[1];
            //TESTPURPOSES
            //Get page of the current username in the database
            var currentScreenSettingID = (from p in db.ScreenSettings
                                          where p.Screen.computer_name == userName
                                          select p.id).First();

            //get the newpage from the database
            var newPageSetting = (from p in db.Pages
                                  where newPage.Contains(p.page_name)
                                  select p).First();

            //find and edit the screensetting of this screen
            ScreenSetting screenSetting = db.ScreenSettings.Find(currentScreenSettingID);
            screenSetting.Page = newPageSetting;
            db.Entry(screenSetting).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", newPage);
        }
    }
}
