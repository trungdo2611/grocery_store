using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        public ActionResult Index(int? page)
        {
            List<BlogViewModel> model = null;
            model = (from blog in db.Blogs
                     join user in db.UserTables on blog.UserID equals user.ID
                     select new BlogViewModel()
                     {
                         blogs = blog,
                         userName = user.UserName
                     })
             .OrderByDescending(c => c.blogs.id)
             .ToList();
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detail(int id)
        {
            Blog detailsModel = db.Blogs.Find(id);
            return View(detailsModel);
        }
    }
}