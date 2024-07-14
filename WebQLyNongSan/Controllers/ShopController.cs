using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Helper;
using WebQLyNongSan.Models;
using PagedList;
using System.Web.UI;
using System.Data.Entity;

namespace WebQLyNongSan.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
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

        public ActionResult home() 
        {
           ShopViewModel1 model = new ShopViewModel1();
            model._products = db.Products.ToList();
            model._categories = db.Categories.ToList();
            // Tính giá trị trung bình đánh giá cho từng sản phẩm
            model.AverageRatings = model._products.ToDictionary(
                product => product.id,
                product => db.Reviews
                               .Where(r => r.ProductID == product.id)
                               .Select(r => (decimal?)r.Rating)
                               .Average()
            );
            return View(model);     
        }

        public ActionResult Index(string searchString,string currentFilter, int? page)
        {
            List<ShopProductViewModel> model = null;
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
                var products = (from product in db.Products
                                join category in db.Categories on product.CategoryId equals category.id
                                join supplier in db.Suppliers on product.supplierId equals supplier.id
                                select new ShopProductViewModel()
                                {
                                    Products = product,
                                    CategoryName = category.Name,
                                    SupplierName = supplier.Name,
                                    StarAverage = (int?)db.Reviews
                                   .Where(r => r.ProductID == product.id)
                                   .Average(r => r.Rating)
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
                              
                         select new ShopProductViewModel()
                         {
                             Products = product,
                             CategoryName = category.Name,
                             SupplierName = supplier.Name,
                             StarAverage = (int?)db.Reviews
                                   .Where(r => r.ProductID == product.id)
                                   .Average(r => r.Rating)  // Cast để tránh exception nếu không có đánh giá
                         })                     
                       
               .OrderByDescending(c => c.Products.id)
               .ToList();
                
            }
            ViewBag.currentFilter = searchString;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CateProduct(int id,int? page)
        {
            //var model = db.Products.Where(n=>n.CategoryId == id).ToList();
            var model = (from product in db.Products 
                         where product.CategoryId == id 
                         select  new CateProductViewModel() 
                         {
                            products =  product,
                            StarAverage = (int?)db.Reviews
                                   .Where(r => r.ProductID == product.id)
                                   .Average(r => r.Rating)
                         }).ToList();
            if (page == null) page = 1;
            model = model.OrderByDescending(n=>n.products.id).ToList();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
            
        }

        public ActionResult SuppProduct(int id, int? page)
        {
            var model = (from product in db.Products
                         where product.supplierId == id
                         select new SuppProductViewModel()
                         {
                             products = product,
                             StarAverage = (int?)db.Reviews
                                   .Where(r => r.ProductID == product.id)
                                   .Average(r => r.Rating)
                         }).ToList();
            if (page == null) page = 1;
            model = model.OrderByDescending(n => n.products.id).ToList();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Detail(int id)
        {
            Product detailsModel = db.Products.Find(id);

            // Lấy các sản phẩm khác trong cùng danh mục
            var relatedProducts = db.Products
                .Where(p => p.CategoryId == detailsModel.CategoryId && p.id != id)
                .Take(5)
                .ToList();
         
            var viewModel = new ProductDetailViewModel()
            {
                DetailsModel = detailsModel,
                StarAverage = (int?)db.Reviews
                                   .Where(r => r.ProductID == id)
                                   .Average(r => r.Rating),
                RelatedProducts = relatedProducts,
            
            };
            ViewBag.productId = id;
            return View(viewModel);
        }

       
    }
}