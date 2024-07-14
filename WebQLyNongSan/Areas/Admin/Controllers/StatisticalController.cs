using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using PagedList;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class StatisticalController : BaseController
    {
        // GET: Admin/Statistical
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index()
        {
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
        public ActionResult TonKho(string searchString, string currentFilter, int? page)
        {
            List<Product> model = null;
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
                var products = db.Products.ToList();
                model = products
                       .Where(c => ConvertToUnSign(c.Name).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                       .OrderByDescending(n => n.id)
                       .ToList();
            } else
            {
                model = db.Products.OrderByDescending(n => n.id).ToList();
            }
            ViewBag.currentFilter = searchString;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }
        
        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.OrderTables
                        join od in db.OrderDetails
                        on o.id equals od.OrderID
                        join p in db.Products
                        on od.ProductID equals p.id
                        select new
                        {
                            Create_at = o.Create_at,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            Cost = p.Cost
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.Create_at >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                
                query = query.Where(x => x.Create_at < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.Create_at)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.Cost),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

       
    }
}