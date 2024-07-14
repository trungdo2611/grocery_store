using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Controllers
{
    public class AuthController : BaseController
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        // GET: Auth
        [HttpGet]
        public ActionResult EditProfile(int id)
        {
            UserTable model = db.UserTables.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfile(UserTable model, HttpPostedFileBase avatar)
        {
            var updateModel = db.UserTables.Find(model.ID);
            if (ModelState.IsValid)
            {
                updateModel.Email = model.Email;
                updateModel.FullName = model.FullName;
                updateModel.Address = model.Address;
                updateModel.Phone = model.Phone;
                if (avatar != null && avatar.ContentLength > 0)
                {
                    string filePath = Server.MapPath("~/Areas/Admin/Image/");
                    string fileName = Path.GetFileName(avatar.FileName);
                    string path = Path.Combine(filePath, fileName);
                    avatar.SaveAs(path);
                    model.avatar = fileName;
                    updateModel.avatar = model.avatar;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Console.WriteLine(e);
                }

                // Lưu thông báo vào Session
                Session["successProfile"] = "Bạn đã chỉnh sửa tài khoản thành công!!";

            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangePassword() { return View(); }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var currentUsername = Session["userName"].ToString();
            var currentPassword = model.CurrentPassword;

            var user = db.UserTables.FirstOrDefault(s => s.UserName == currentUsername && s.PassWord == currentPassword);
            if (user == null)
            {
                // user password did not match the DB password
                TempData["errorChangePass"] = "Mật khẩu không đúng";
                return View();
            }
            else
            {
                // update the new password
                user.PassWord = model.NewPassword;
                // save the Database changes
                db.SaveChanges();
            }
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}