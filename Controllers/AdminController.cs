﻿﻿﻿﻿﻿using FoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWeb.Controllers
{
    public class AdminController : Controller
    {
        AppFoodDbContext db = new AppFoodDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if(adminInCookie != null)
            {
                ViewBag.UserCount = db.SignupLogin.Count();      
                ViewBag.OrderCount = db.orders.Count();
                return View();
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    return RedirectToAction("LoginAdmin", "Admin");
                }
            }

        }
        [HttpGet]
        public ActionResult LoginAdmin()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                return RedirectToAction("Index", "Admin"); ;
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    return View();
                }
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(AdminLogin model)
        {
            var data = db.adminLogin.Where(s => s.Email.Equals(model.Email) && s.Password.Equals(model.Password)).ToList();
            if (data.Count() > 0)
            {
                HttpCookie cooskie = new HttpCookie("AdminInfo");
                cooskie.Values["idAdmin"] = Convert.ToString(data.FirstOrDefault().adminid);
                cooskie.Values["Email"] = Convert.ToString(data.FirstOrDefault().Email);
                cooskie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(cooskie);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.Message = "Login failed";
                return RedirectToAction("LoginAdmin");
            }
        }
        public ActionResult LogoutAdmin()
        {
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("AdminInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["AdminInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("LoginAdmin");
        }

        public ActionResult ListOfOrders()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                float t = 0;
                List<Order> order = db.orders.ToList<Order>();
                foreach(var item in order)
                {
                    t += item.Order_Bill;
                }
                TempData["OrderTotal"] = t;
                return View(order);
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    return RedirectToAction("LoginAdmin", "Admin");
                }
            }
        }
        public ActionResult ListOfInvoices()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                float t = 0;
                List<InvoiceModel> invoice = db.invoiceModel.ToList<InvoiceModel>();
                
                foreach (var item in invoice)
                {
                    t += item.Total_Bill;
                   
                    
                }
                TempData["InvoiceTotal"] = t;
                return View(invoice);
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    return RedirectToAction("LoginAdmin", "Admin");
                }
            }
        }
        public ActionResult UserList()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                var users = db.SignupLogin.ToList();
                return View(users);
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        // Blog Management Actions
        public ActionResult BlogList()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                var blogs = db.blogModels.OrderByDescending(b => b.BlogDate).ToList();
                return View(blogs);
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        [HttpGet]
        public ActionResult CreateBlog()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBlog(BlogModel blog)
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // Ensure the blog date is set
                        blog.BlogDate = DateTime.Now;
                        
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(blog.BlogTitle))
                        {
                            ModelState.AddModelError("BlogTitle", "Blog title is required.");
                            return View(blog);
                        }
                        
                        if (string.IsNullOrWhiteSpace(blog.BlogAutherName))
                        {
                            ModelState.AddModelError("BlogAutherName", "Author name is required.");
                            return View(blog);
                        }
                        
                        if (string.IsNullOrWhiteSpace(blog.BlogDescription))
                        {
                            ModelState.AddModelError("BlogDescription", "Blog description is required.");
                            return View(blog);
                        }
                        
                        // Add to database
                        db.blogModels.Add(blog);
                        db.SaveChanges();
                        
                        TempData["Success"] = "Blog created successfully!";
                        return RedirectToAction("BlogList");
                    }
                    else
                    {
                        // Add detailed model state errors to TempData for debugging
                        var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                                                .ToList();
                        
                        TempData["ModelStateErrors"] = string.Join("; ", 
                            errors.SelectMany(e => e.Errors.Select(msg => $"{e.Field}: {msg}")));
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception and show user-friendly error
                    TempData["Error"] = "An error occurred while creating the blog post. Please try again.";
                    TempData["ErrorDetails"] = ex.Message; // For debugging, remove in production
                }
                
                return View(blog);
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditBlog(int id)
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                var blog = db.blogModels.Find(id);
                if (blog == null)
                {
                    return HttpNotFound();
                }
                return View(blog);
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBlog(BlogModel blog)
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(blog).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Blog updated successfully!";
                    return RedirectToAction("BlogList");
                }
                return View(blog);
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        [HttpGet]
        public ActionResult DeleteBlog(int id)
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                var blog = db.blogModels.Find(id);
                if (blog != null)
                {
                    db.blogModels.Remove(blog);
                    db.SaveChanges();
                    TempData["Success"] = "Blog deleted successfully!";
                }
                return RedirectToAction("BlogList");
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        // Method to add sample blog content
        public ActionResult AddSampleBlogs()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                // Check if blogs already exist
                if (!db.blogModels.Any())
                {
                    var sampleBlogs = new List<BlogModel>
                    {
                        new BlogModel
                        {
                            BlogTitle = "Top 10 Popular Foods in Bangladesh",
                            BlogAutherName = "FoodSync Team",
                            BlogDate = DateTime.Now.AddDays(-5),
                            BlogDescription = "Bangladesh has a rich culinary heritage with diverse flavors and traditional recipes. From aromatic biryanis to spicy curries, Bangladeshi cuisine offers something for every palate. In this post, we explore the most beloved dishes that define our food culture. Biryani tops our list with its fragrant basmati rice and tender meat, followed by hilsa fish curry - the national fish prepared in countless delicious ways. Bhuna khichuri, a comfort food during monsoons, combines rice and lentils with aromatic spices. Pitha, traditional rice cakes, are perfect for winter evenings. Street foods like fuchka (pani puri), jhal muri, and chotpoti represent the vibrant street food culture. These dishes reflect the heart and soul of Bangladeshi cooking, passed down through generations."
                        },
                        new BlogModel
                        {
                            BlogTitle = "Healthy Eating During Ramadan: A Complete Guide",
                            BlogAutherName = "Dr. Nutritionist",
                            BlogDate = DateTime.Now.AddDays(-3),
                            BlogDescription = "Ramadan is a holy month of fasting, but it's also important to maintain proper nutrition during sehri and iftar. This comprehensive guide will help you make healthy food choices that keep you energized throughout the day. Start your sehri with complex carbohydrates like oats or whole grain bread, paired with protein from eggs or yogurt. Include dates for natural sugars and plenty of water for hydration. For iftar, break your fast with dates and water, then proceed to a balanced meal with lean proteins, vegetables, and whole grains. Avoid fried and overly sugary foods that can cause energy crashes. Traditional items like haleem provide excellent protein and fiber. Remember to drink plenty of water between iftar and sehri to stay hydrated."
                        },
                        new BlogModel
                        {
                            BlogTitle = "Quick 15-Minute Meals for Busy Professionals",
                            BlogAutherName = "Chef Rahman",
                            BlogDate = DateTime.Now.AddDays(-2),
                            BlogDescription = "In today's fast-paced world, finding time to cook nutritious meals can be challenging. Here are some quick and easy recipes that take only 15 minutes to prepare, perfect for busy professionals. Egg fried rice with vegetables provides complete nutrition and uses leftover rice. Chicken stir-fry with mixed vegetables offers protein and vitamins in one dish. Lentil soup (dal) with rice is a traditional, nutritious combination that's quick to prepare. Vegetable sandwiches with hummus provide plant-based protein and fiber. For seafood lovers, fish curry with minimal ingredients can be prepared quickly using pre-marinated fish. These recipes prove that healthy eating doesn't require hours in the kitchen. Keep your pantry stocked with basic spices, canned beans, and frozen vegetables for emergency meals."
                        },
                        new BlogModel
                        {
                            BlogTitle = "Meet Our Partner Restaurants: Chef's Kitchen Story",
                            BlogAutherName = "FoodSync Team",
                            BlogDate = DateTime.Now.AddDays(-1),
                            BlogDescription = "Today we spotlight Chef's Kitchen, one of our valued restaurant partners who has been serving authentic Bangladeshi cuisine for over 15 years. Located in the heart of Dhaka, this family-owned restaurant started as a small venture by Chef Karim and his wife. Their commitment to using fresh, locally-sourced ingredients and traditional cooking methods has made them a favorite among food lovers. Chef Karim shares his journey: 'We started with just five tables and a dream to serve authentic home-style cooking. Every dish is prepared with love and the same care we would give to our own family.' Their signature dishes include slow-cooked beef rezala, aromatic chicken biryani, and traditional fish curry. The restaurant employs local cooks and sources vegetables from nearby farmers, supporting the community while maintaining quality. This partnership allows us to bring you restaurant-quality meals delivered fresh to your door."
                        },
                        new BlogModel
                        {
                            BlogTitle = "Benefits of Bengali Spices: Nature's Medicine Cabinet",
                            BlogAutherName = "Health Expert",
                            BlogDate = DateTime.Now,
                            BlogDescription = "Bengali cuisine is renowned for its aromatic spices, but did you know these spices offer incredible health benefits? Turmeric, the golden spice, contains curcumin which has powerful anti-inflammatory properties and boosts immunity. Cumin aids digestion and helps regulate blood sugar levels. Cardamom, often called the 'queen of spices,' improves heart health and has antioxidant properties. Coriander seeds help lower cholesterol and support digestive health. Fenugreek seeds can help manage diabetes and improve heart health. Ginger, commonly used in Bengali cooking, reduces nausea and has anti-inflammatory effects. Garlic boosts immune function and may help lower blood pressure. These spices have been used in traditional medicine for centuries, and modern science continues to validate their health benefits. Incorporating these spices into your daily meals not only enhances flavor but also contributes to overall wellness."
                        }
                    };

                    db.blogModels.AddRange(sampleBlogs);
                    db.SaveChanges();
                    TempData["Success"] = "Sample blog content has been added successfully!";
                }
                else
                {
                    TempData["Info"] = "Blog content already exists.";
                }
                return RedirectToAction("BlogList");
            }
            else
            {
                return RedirectToAction("LoginAdmin");
            }
        }

        // AJAX method to get blog count
        public JsonResult GetBlogCount()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                var count = db.blogModels.Count();
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

    }
}