using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using WebQLyNongSan.Helper;
using System.IO;
using PagedList;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        // GET: Admin/User
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
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
            List<UserViewModel> model = null;
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
                var us = (from user in db.UserTables
                          join role in db.Roles on user.RoleID equals role.id
                          select new UserViewModel()
                          {
                              User = user,
                              RoleName = role.Name,
                            
                          })
                .ToList();
                model = us
                .Where(c =>
                        ConvertToUnSign(c.User.UserName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ConvertToUnSign(c.RoleName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0             
                         )
                .ToList();
            }
            else
            {
               model = (from user in db.UserTables
                        join role in db.Roles on user.RoleID equals role.id
                        select new UserViewModel()
                        {
                            User = user,
                            RoleName = role.Name,

                        }).OrderByDescending(c => c.User.ID)
                .ToList();
            }
            ViewBag.currentFilter = searchString;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            UserTable model = db.UserTables.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserTable model, HttpPostedFileBase avatar)
        {
            var updateModel = db.UserTables.Find(model.ID);
            if (ModelState.IsValid)
            {
                updateModel.UserName = model.UserName;
                updateModel.Email= model.Email;
                updateModel.RoleID = model.RoleID;
                updateModel.FullName= model.FullName;
                updateModel.Address = model.Address;
                updateModel.Phone= model.Phone;
                updateModel.PassWord= model.PassWord;
                if (avatar != null && avatar.ContentLength > 0)
                {
                    string filePath = Server.MapPath("~/Areas/Admin/Image/");
                    string fileName = Path.GetFileName(avatar.FileName);
                    string path = Path.Combine(filePath, fileName);
                    avatar.SaveAs(path);
                    model.avatar = fileName;
                    updateModel.avatar = model.avatar;
                }
                db.SaveChanges();

                // Lưu thông báo vào Session
                Session["SuccessMessage"] = "tài khoản";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.UserTables.SingleOrDefault(m => m.ID == id);
            db.UserTables.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            UserTable detailsModel = db.UserTables.Find(id);
            return View(detailsModel);
        }

    }
}