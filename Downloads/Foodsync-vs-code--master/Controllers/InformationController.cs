using FoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWeb.Controllers
{
    public class InformationController : Controller
    {
        AppFoodDbContext db = new AppFoodDbContext();
        // GET: Information
        public ActionResult ContactUs(ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                db.contactModels.Add(contact);
                db.SaveChanges();

               
                TempData["Success"] = "Your message has been sent successfully!";

               
                return RedirectToAction("ContactUs");
            }

            
            return View(contact);
        }
        public ActionResult MessageList()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                List<ContactModel> contacts = db.contactModels.ToList<ContactModel>();
                return View(contacts);
                
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    return RedirectToAction("Products", "Index");

                }
                else
                {
                    return RedirectToAction("LoginAdmin", "Admin");
                }
            }
        }
        public ActionResult AboutUs()
        {

            return View();
        }
        public ActionResult Blogs()
        {

            return View();
        }

    }
}