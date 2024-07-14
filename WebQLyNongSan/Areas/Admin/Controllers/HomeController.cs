using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index()
        {
            var model = db.OrderTables.OrderByDescending(n=>n.id).Take(6).ToList();
            return View(model);
        }

        public ActionResult partialIndex()
        {
            var model = db.UserTables.OrderByDescending(n => n.ID).Take(6).ToList();
            return PartialView(model);
        }
    }
}