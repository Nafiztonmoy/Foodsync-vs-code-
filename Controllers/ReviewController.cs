using FoodWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWeb.Controllers
{
    public class ReviewController : Controller
    {
        private AppFoodDbContext db = new AppFoodDbContext();

        // GET: /Review
        [Route("Review")]
        public ActionResult Index()
        {
            var reviews = db.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
            return View(reviews);
        }

        // POST: /Review/Add
        [HttpPost]
        [Route("Review/Add")]
        public ActionResult Add(ReviewModel model, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Reviews"), fileName);
                    Directory.CreateDirectory(Server.MapPath("~/Uploads/Reviews"));
                    ImageFile.SaveAs(path);
                    model.ImagePath = "/Uploads/Reviews/" + fileName;
                }

                model.CreatedAt = DateTime.Now;
                db.Reviews.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var reviews = db.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
            return View("Index", reviews);
        }
    }
}
