using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using System.IO;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class SupplierController : BaseController
    {
        // GET: Admin/Supplier
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index()
        {
            if (Session["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = Session["SuccessMessage"].ToString();
                Session["SuccessMessage"] = null; // Xóa thông báo từ Session
            }
            return View();
        }

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
        public JsonResult GetData(string searchString, int? page, int? pageSize)
        {
            List<Supplier> model = null;
            if (!string.IsNullOrEmpty(searchString))
            {
               string searchKeyword = ConvertToUnSign(searchString);
                // Thực hiện tìm kiếm gần đúng
                var suppliers = db.Suppliers.ToList();
                model = suppliers
                   .Where(c => ConvertToUnSign(c.Name).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                   .ToList();

            }
            else
            {
                model = db.Suppliers.OrderByDescending(n => n.id).ToList();
                
            }

            var _pageSize = pageSize ?? 4;
            var pageIndex = page ?? 1;
            var totalPage = model.Count;
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            var start = (pageIndex - 1) * _pageSize;
            model = model.Skip(start).Take(_pageSize).ToList();

            ViewBag.searchString = searchString;
            var result = new { datas = model, currrentPage = pageIndex, total = totalPage, NumberPage = numberPage,PageSize = _pageSize };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(Supplier model)
        {
            if (ModelState.IsValid)
            {             
                db.Suppliers.Add(model);
                db.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            Supplier model = db.Suppliers.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Supplier model)
        {
            var updateModel = db.Suppliers.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.Name = model.Name;
                updateModel.Address = model.Address;
                updateModel.Phone = model.Phone;
                db.SaveChanges();

                // Lưu thông báo vào Session
                Session["SuccessMessage"] = "nhà sản xuất";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.Suppliers.SingleOrDefault(m => m.id == id);
            db.Suppliers.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Supplier detailsModel = db.Suppliers.Find(id);
            return View(detailsModel);
        }
    }
}