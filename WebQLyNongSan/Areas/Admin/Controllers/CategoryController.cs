using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using WebQLyNongSan.Models;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Xml.Schema;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        // GET: Admin/Category
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
        public JsonResult GetCategory(string searchString, int? page, int? pageSize)
        {
            
            List<Category> model = null;
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchKeyword = ConvertToUnSign(searchString);
                var categories = db.Categories.ToList(); // Lấy danh sách tất cả danh mục
                                                         // Thực hiện tìm kiếm gần đúng
              model = categories
                    .Where(c => ConvertToUnSign(c.Name).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(n => n.id)
                    .ToList();               
            }
            else
            {
                model = db.Categories.OrderByDescending(n => n.id).ToList();

            }
            var _pageSize = pageSize ?? 4;
            var pageIndex = page ?? 1;
            var totalPage = model.Count;
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            var start = (pageIndex - 1) * _pageSize;
            model = model.Skip(start).Take(_pageSize).ToList();
            ViewBag.searchString = searchString;
            var result = new { datas = model, currrentPage = pageIndex, total = totalPage, NumberPage = numberPage, PageSize = _pageSize };
            return Json(result, JsonRequestBehavior.AllowGet); 
        }

        

        [HttpPost]
        public ActionResult Create(Category model, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture != null && Picture.ContentLength > 0)
                {
                    string filePath = Server.MapPath("~/Areas/Admin/Image/");
                    string fileName = Path.GetFileName(Picture.FileName);
                    model.Picture = fileName;
                    string path = Path.Combine(filePath, fileName);
                    Picture.SaveAs(path);
                }

                db.Categories.Add(model);
                db.SaveChanges();
            }


            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Category model = db.Categories.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Category model, HttpPostedFileBase Picture)
        {
            var updateModel = db.Categories.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.Name = model.Name;
                updateModel.Alias = model.Alias;
                if (Picture != null && Picture.ContentLength > 0)
                {
                    string filePath = Server.MapPath("~/Areas/Admin/Image/");
                    string fileName = Path.GetFileName(Picture.FileName);
                    string path = Path.Combine(filePath, fileName);
                    Picture.SaveAs(path);
                    model.Picture = fileName;
                    updateModel.Picture = model.Picture;
                }
                db.SaveChanges();

                // Lưu thông báo vào Session
                Session["SuccessMessage"] = "danh mục";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.Categories.SingleOrDefault(m => m.id == id);
            db.Categories.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            
            Category detailsModel = db.Categories.Find(id);
            return View(detailsModel);
        }
    }

}