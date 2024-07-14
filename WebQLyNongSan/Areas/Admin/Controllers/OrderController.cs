using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Admin/Order
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
            if (Session["successEdit"] != null)
            {
                ViewBag.successEdit = Session["successEdit"].ToString();
                Session["successEdit"] = null; // Xóa thông báo từ Session
            }

            List<OrderManageViewModel> model = null;
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
                var us = (from order in db.OrderTables select new OrderManageViewModel
                {
                    orders = order,
                }).ToList();

                model = us
                .Where(c =>
                        ConvertToUnSign(c.orders.Code).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ConvertToUnSign(c.orders.FullName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0                                         
                         )
                .ToList();
            }
            else
            {
                model = (from order in db.OrderTables                      
                         select new OrderManageViewModel()
                         {                           
                             orders = order,                            
                         }).OrderByDescending(c => c.orders.id)
                 .ToList();

            }
            ViewBag.currentFilter = searchString;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Details(int id)
        {       
           var model = (from orD in db.OrderDetails 
                        where orD.OrderID== id 
                        select orD).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            OrderTable model = db.OrderTables.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(OrderTable model)
        {
            var updateModel = db.OrderTables.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.StransactStatusId = model.StransactStatusId;
                
                db.SaveChanges();

                // Lưu thông báo vào Session
                Session["successEdit"] = "trạng thái của đơn hàng";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.OrderTables.SingleOrDefault(m => m.id == id);
            db.OrderTables.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}