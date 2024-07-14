using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        // GET: Admin/Auth
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserName, string PassWord)
        {
            var data = db.UserTables.SingleOrDefault(x => x.UserName.Equals(UserName) && x.PassWord.Equals(PassWord));
            if (data != null)
            {
                if(data.RoleID== 1) 
                {
                    Session["adminName"] = data.UserName;
                    Session["avatar"] = data.avatar;
                    Session["adminId"] = data.ID;
                    return RedirectToAction("Index", "Home");
                }else
                {
                    TempData["msgRole"] = "Tài khoản của bạn không đủ quyền truy cập.";
                    return View();
                }
                
            }
            else
            {
                TempData["msg"] = "Tên đăng nhập hoặc mật khẩu không đúng";              
            }  
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login","Auth");
        }
    }
}