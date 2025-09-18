using FoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWeb.Controllers
{
    public class UserController : Controller
    {
        AppFoodDbContext db = new AppFoodDbContext();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Signup()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                return RedirectToAction("Index", "Products");
            }
            else
            {
                var adminInCookie = Request.Cookies["AdminInfo"];
                if (adminInCookie != null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return View();
                }

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(SignupLogin signup)
        {
            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = db.SignupLogin.Any(x => x.Email == signup.Email);
                if (isEmailAlreadyExists)
                {
                    ViewBag.Message = "Email Already Registered. Please Try Again With Another Email";
                    return View();
                }
                else
                {
                    db.SignupLogin.Add(signup);
                    db.SaveChanges();
                    TempData["Success"] = "Registration successful!";
                    return RedirectToAction("Index", "Products");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                return RedirectToAction("Index", "Products");
            }
            else
            {
                var adminInCookie = Request.Cookies["AdminInfo"];
                if (adminInCookie != null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return View();
                }
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SignupLogin model)
        {
            var data = db.SignupLogin.Where(s => s.Email.Equals(model.Email) && s.Password.Equals(model.Password)).ToList();
            if (data.Count() > 0)
            {
                Session["uid"] = data.FirstOrDefault().userid;
                HttpCookie cooskie = new HttpCookie("UserInfo");
                cooskie.Values["idUser"] = Convert.ToString(data.FirstOrDefault().userid);
                cooskie.Values["FullName"] = Convert.ToString(data.FirstOrDefault().Name);
                cooskie.Values["Email"] = Convert.ToString(data.FirstOrDefault().Email);
                cooskie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(cooskie);
                return RedirectToAction("Index", "Products");
            }
            else
            {
                ViewBag.Message = "Login failed";
                return RedirectToAction("Login");
            }
        }
        public ActionResult Logout()
        {

            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["UserInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            Session.Clear();
            return RedirectToAction("Login");
        }
        // GET: UserProfile
        public ActionResult EditProfile()
        {
            // Get logged-in user ID from session or cookie
            int userId = Convert.ToInt32(Session["uid"]);
            var user = db.SignupLogin.Find(userId);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(SignupLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.SignupLogin.Find(model.userid);
            if (user == null) return HttpNotFound();

            // Update fields
            user.Name = model.Name;
            user.Address = model.Address;
            user.PhoneNo = model.PhoneNo;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = model.Password; // ⚠️ should hash in real apps
                user.ConfirmPassword = model.ConfirmPassword;
            }

            db.SaveChanges();
            TempData["Success"] = "Profile updated successfully!";

            return RedirectToAction("Index", "Products"); // or a profile page
        }

        [HttpGet]
        public ActionResult ViewProfile()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = Convert.ToInt32(Session["uid"]);
            var user = db.SignupLogin.FirstOrDefault(u => u.userid == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


    }
}