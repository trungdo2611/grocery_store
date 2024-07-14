using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        QLyNongSanEntities2 db = new QLyNongSanEntities2();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(Contact model)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(model);
                try
                {
                    db.SaveChanges();
                } catch (DbEntityValidationException e)
                {
                    Console.WriteLine(e);
                }
                
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}