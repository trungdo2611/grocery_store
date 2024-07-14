using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        QLyNongSanEntities2 db = new QLyNongSanEntities2();

        public ActionResult Index()
        {
          
            return View();
        }
        public ActionResult PartialIndexReview(int id)
        {
            if (Session["userId"] == null)
            {
               return RedirectToAction("Index", "Home"); ;
            }
            var model = new List<ReviewViewModel>();
           model = (from review in db.Reviews
                         join user in db.UserTables on review.UserID equals user.ID
                         where review.ProductID == id
                         select new ReviewViewModel()
                         {
                          reviews = review,
                          userName = user.UserName,
                          avatarUser = user.avatar,
                         })
                .OrderByDescending(c=>c.reviews.id)
                .ToList();
            return PartialView("PartialIndexReview",model);
        }

        [HttpPost]
        public JsonResult CreateComment(Review model)
        {
            if (ModelState.IsValid)
            {
                if(model.Rating == null)
                {
                    TempData["errorRating"] = "Bạn cần phải đánh giá ít nhất một sao để bình luận";
                }
                string userName = Session["userName"].ToString();
                model.Create_at = DateTime.Now;
                model.UserID = db.UserTables.Single(a => a.UserName == userName).ID;   
                db.Reviews.Add(model);
                try
                {
                db.SaveChanges();
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }


    }
}