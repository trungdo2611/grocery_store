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
    public class HomeController : Controller
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index()
        {
            if (Session["successRegister"] != null)
            {
                ViewBag.successRegister = Session["successRegister"].ToString();
                Session["successRegister"] = null; // Xóa thông báo từ Session
            }
            if (Session["successProfile"] != null)
            {
                ViewBag.successProfile = Session["successProfile"].ToString();
                Session["successProfile"] = null; // Xóa thông báo từ Session
            }
            return View();
        }

   
        [HttpPost]
        public ActionResult Login(string UserName, string PassWord)
        {
            var status = 0;
            var data = db.UserTables.SingleOrDefault(x => x.UserName.Equals(UserName) && x.PassWord.Equals(PassWord));
            if (data != null)
            {
                Session["userName"] = data.UserName;
                Session["avatarUser"] = data.avatar;
                Session["userId"] = data.ID;
                status = 1;
            }
            else
            {
                TempData["msgLogin"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                status = 0;
               
            }
            var result = new { Status = status};
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register() { return View(); }

        [HttpPost]
        public ActionResult Register(UserTable model, HttpPostedFileBase avatar)
        {
           
                var check = db.UserTables.FirstOrDefault(s => s.UserName.ToLower() == model.UserName.ToLower());
                if (check == null)
                {
                    if(ModelState.IsValid)
                    {
                        if (avatar != null && avatar.ContentLength > 0)
                        {
                            string filePath = Server.MapPath("~/Areas/Admin/Image/");
                            string fileName = Path.GetFileName(avatar.FileName);
                            model.avatar = fileName;
                            string path = Path.Combine(filePath, fileName);
                            avatar.SaveAs(path);
                        }
                        model.RoleID = 2;
                        model.Active = true;
                        db.UserTables.Add(model);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        Console.WriteLine(e);
                    }
                        // Lưu thông báo vào Session
                        Session["successRegister"] = "Bạn đã tạo tài khoản thành công, hãy đăng nhập";
                        return RedirectToAction("Index", "Home");
                    }
                } else
                {
                    ViewBag.errorUserName = "Tên tài khoản đã được sử dụng";  
                }
            return View();
        }

        
    }
}