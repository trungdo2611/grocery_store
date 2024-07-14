using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using PagedList;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class ContactController : BaseController
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        // GET: Admin/Contact

        private string ConvertToUnSign(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            return str2;
        }
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            List<Contact> model = null;
            if (Session["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = Session["SuccessMessage"].ToString();
                Session["SuccessMessage"] = null; // Xóa thông báo từ Session
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchKeyword = ConvertToUnSign(searchString);
                // Thực hiện tìm kiếm gần đúng
                var contacts = db.Contacts.ToList();
                model = contacts
                   .Where(c => ConvertToUnSign(c.Name).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                   .ToList();

            }
            else
            {
                model = db.Contacts.OrderByDescending(n => n.id).ToList();

            }
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            ViewBag.currentFilter = searchString;
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Contact model = db.Contacts.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Contact model)
        {
            var updateModel = db.Contacts.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.Name = model.Name;
                updateModel.Email = model.Email;
                updateModel.Phone = model.Phone;
                updateModel.Content= model.Content;
                db.SaveChanges();

                // Lưu thông báo vào Session
                Session["SuccessMessage"] = "liên hệ";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.Contacts.SingleOrDefault(m => m.id == id);
            db.Contacts.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Contact detailsModel = db.Contacts.Find(id);
            return View(detailsModel);
        }
    }
}