using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using System.IO;
using WebQLyNongSan.Helper;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class BlogController : BaseController
    {
        // GET: Admin/Blog
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

        public ActionResult GetData(string searchString, int? page, int? pageSize)
        {
            List<BlogAdminViewModel> model = null;
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchKeyword = ConvertToUnSign(searchString);
                var blogs = (from blog in db.Blogs
                                join user in db.UserTables on blog.UserID equals user.ID                            
                                select new BlogAdminViewModel()
                                {
                                    blogs = blog,
                                   userName = user.UserName
                                })
                .ToList();
                model = blogs
                .Where(c =>
                        ConvertToUnSign(c.blogs.Title).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ConvertToUnSign(c.userName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 
                     
                         )
                .ToList();

            }
            else
            {
                model = (from blog in db.Blogs
                         join user in db.UserTables on blog.UserID equals user.ID
                         select new BlogAdminViewModel()
                         {
                             blogs = blog,
                             userName = user.UserName
                         })
               .OrderByDescending(c => c.blogs.id)
               .ToList();
            }
            var _pageSize = pageSize ?? 4;
            var pageIndex = page ?? 1;
            var totalPage = model.Count;
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            var start = (pageIndex - 1) * _pageSize;
            model = model.Skip(start).Take(_pageSize).ToList();
            var result = new { datas = model, currrentPage = pageIndex, total = totalPage, NumberPage = numberPage, PageSize = _pageSize };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Create(Blog model, HttpPostedFileBase Picture)
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
                model.Create_at = DateTime.Now;
                model.UserID = (int?)Session["adminId"];
                db.Blogs.Add(model);
                db.SaveChanges();
            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Blog model = db.Blogs.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Blog model, HttpPostedFileBase Picture)
        {
            var updateModel = db.Blogs.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.Title = model.Title;
                updateModel.Content = model.Content;
                model.Create_at = DateTime.Now;
                updateModel.Create_at = model.Create_at;
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
                Session["SuccessMessage"] = "bài viết";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.Blogs.SingleOrDefault(m => m.id == id);
            db.Blogs.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Blog detailsModel = db.Blogs.Find(id);
            return View(detailsModel);
        }
    }
}