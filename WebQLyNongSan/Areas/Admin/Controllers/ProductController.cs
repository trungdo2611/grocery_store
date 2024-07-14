using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using System.Drawing.Printing;
using System.Web.UI;
using WebQLyNongSan.Helper;
using System.IO;

namespace WebQLyNongSan.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Admin/Product
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
            List<ProductViewModel> model = null;
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchKeyword = ConvertToUnSign(searchString);
                var products = (from product in db.Products
                                join category in db.Categories on product.CategoryId equals category.id
                                join supplier in db.Suppliers on product.supplierId equals supplier.id
                                select new ProductViewModel()
                                {
                                    Products = product,
                                    CategoryName = category.Name,
                                    SupplierName = supplier.Name,
                                })
                .ToList();
                model = products
                .Where(c =>
                        ConvertToUnSign(c.Products.Name).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ConvertToUnSign(c.CategoryName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                         ConvertToUnSign(c.SupplierName).IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0
                         )
                .ToList();

            }
            else
            {
                model = (from product in db.Products
                         join category in db.Categories on product.CategoryId equals category.id
                         join supplier in db.Suppliers on product.supplierId equals supplier.id

                         select new ProductViewModel()
                         {
                             Products = product,
                             CategoryName = category.Name,
                             SupplierName = supplier.Name,
                         })

               .OrderByDescending(c => c.Products.id)
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
        public ActionResult Create(Product model, HttpPostedFileBase Picture)
        {
            if(ModelState.IsValid)
            {

                if (Picture != null && Picture.ContentLength > 0)
                {
                    string filePath = Server.MapPath("~/Areas/Admin/Image/");
                    string fileName = Path.GetFileName(Picture.FileName);
                    model.Picture = fileName;
                    string path = Path.Combine(filePath, fileName);
                    Picture.SaveAs(path);
                }

                db.Products.Add(model);
                db.SaveChanges();
            }
            
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Product model = db.Products.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Product model, HttpPostedFileBase Picture)
        {
            var updateModel = db.Products.Find(model.id);
            if (ModelState.IsValid)
            {
                updateModel.Name = model.Name;
                updateModel.Alias = model.Alias;
                updateModel.CategoryId = model.CategoryId;
                updateModel.supplierId= model.supplierId;
                updateModel.ShortDesc = model.ShortDesc;
                updateModel.FullDesc= model.FullDesc;
                updateModel.Cost = model.Cost;
                updateModel.Price = model.Price;
                updateModel.UnitStock= model.UnitStock;
                updateModel.HomeFlag= model.HomeFlag;
                updateModel.BestSeller = model.BestSeller;
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
                Session["SuccessMessage"] = "sản phẩm";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = db.Products.SingleOrDefault(m => m.id == id);
            db.Products.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            Product detailsModel = db.Products.Find(id);
            return View(detailsModel);
        }

    }
}