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
        UnimacsContext db = new UnimacsContext();

        // GET: Settings
        public ActionResult Index()
        {
            foreach(ScreenSetting ss in db.ScreenSettings.ToList())
            {
                List<SelectListItem> selectList = new List<SelectListItem>();
                foreach (Page p in db.Pages.ToList())
                {
                    selectList.Add(new SelectListItem { Text = p.page_name, Value = p.id.ToString(), Selected = (ss.page_id == p.id)});
                }
                ss.SelectListItems = selectList;
            }
            return View(db.ScreenSettings.ToList());
        }

        // POST: Settings/Update
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                List<String> screenSettingIds = Request.Form["id"].Split(',').ToList();
                List<String> screenIds = Request.Form["screen_id"].Split(',').ToList();
                List<String> pageIds = Request.Form["page_id"].Split(',').ToList();

                for(int i = 0; i < screenSettingIds.Count(); i++)
                {
                    int id = 0;
                    int pageId = 0;
                    int screenId = 0;
                    Int32.TryParse(screenSettingIds[i], out id);
                    Int32.TryParse(pageIds[i], out pageId);
                    Int32.TryParse(screenIds[i], out screenId);

                    ScreenSetting screenSetting = db.ScreenSettings.Find(id);
                    screenSetting.Page = db.Pages.Find(pageId);
                    screenSetting.Screen = db.Screens.Find(screenId);
                    screenSetting.timestamp = BitConverter.GetBytes(DateTime.Now.Ticks);
                    db.Entry(screenSetting).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Index", "Settings");
        }
    }
}
