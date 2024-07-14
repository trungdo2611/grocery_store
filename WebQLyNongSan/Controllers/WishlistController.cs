using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Controllers
{
    
    public class WishlistController : BaseController
    {
        // GET: Wishlist
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index(int? page)
        {
            int userId = (int)Session["userId"];
            var items =(from wishs in db.WishLists
                        join products in db.Products on wishs.ProductID equals products.id
                        where wishs.UserID == userId
                        select new WishlistViewModel()
                        {
                            products= products,
                            wishs= wishs,
                        }).ToList();
            items.OrderByDescending(n => n.wishs.id);
            if (page == null) page = 1;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult PostWishlist(int productId)
        {
            int userId = (int)Session["userId"];
            var checkItem = db.WishLists.FirstOrDefault(x=>x.ProductID== productId && x.UserID == userId);
            if (checkItem != null)
            {
                return Json(new {Success = false, msg = "Sản phẩm đã được yêu thích rồi"});
            }
            var item = new WishList();
            item.ProductID = productId;
            item.UserID = userId;
            db.WishLists.Add(item);
            db.SaveChanges();
            return Json(new {Success = true});
            
        }

        [HttpPost]
        public ActionResult DeleteWishlist(int productId)
        {
            int userId = (int)Session["userId"];
            var checkItem = db.WishLists.FirstOrDefault(x => x.ProductID == productId && x.UserID == userId);
            if (checkItem != null)
            {
                db.WishLists.Remove(checkItem);
                db.SaveChanges();
                return Json(new { Success = true, msg = "Xóa thành công" });
            }
            return Json(new { Success = false, msg = "Xóa thất bại" });

        }

    }
}