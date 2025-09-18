using FoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWeb.Controllers
{
    public class ProductsController : Controller
    {
        AppFoodDbContext db = new AppFoodDbContext();
  
        public ActionResult Checkout()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
            if (adminInCookie != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var userInCookie = Request.Cookies["UserInfo"];
                if (userInCookie != null)
                {
                    TempData.Keep();
                    if (TempData["cart"] != null)
                    {
                        float x = 0;
                        List<Cart> li2 = TempData["cart"] as List<Cart>;
                        foreach (var item in li2)
                        {
                            x += item.bill;
                        }
                        TempData["total"] = x;
                    }
                    TempData.Keep();
                    return View();

                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }

           
        }
        [HttpPost]
        public ActionResult Checkout(Order order)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            int iduser = Convert.ToInt32(userInCookie["idUser"]);
            List<Cart> li = TempData["cart"] as List<Cart>;
            InvoiceModel invoice = new InvoiceModel();
            invoice.FKUserID = iduser;
            invoice.DateInvoice = System.DateTime.Now;
            invoice.Total_Bill = (float)TempData["Total"];
            db.invoiceModel.Add(invoice);
            db.SaveChanges();
            foreach(var item in li)
            {
                Order odr = new Order();
                odr.FkProdId = item.productId;
                odr.FkInvoiceID = invoice.ID;
                odr.Order_Date = System.DateTime.Now;
                odr.Qty = item.qty;
                odr.Unit_Price = (int)item.price;
                odr.Order_Bill = item.bill;
                db.orders.Add(odr);
                db.SaveChanges();
            }
            TempData.Remove("total");
            TempData.Remove("cart");
            TempData.Keep();
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int? id)
        {
            List<Cart> li2 = TempData["cart"] as List<Cart>;
            Cart c = li2.Where(x => x.productId == id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;
            foreach(var item in li2)
            {
                h += item.bill;
            }
            TempData["total"] = h;
            return RedirectToAction("Checkout");
        }
        // Add product to wishlist
        public ActionResult AddToWishlist(int id)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                int userId = Convert.ToInt32(userInCookie["idUser"]);

                // check duplicate
                var exists = db.Wishlists.FirstOrDefault(w => w.ProductId == id && w.UserId == userId);
                if (exists == null)
                {
                    Wishlist wishlist = new Wishlist
                    {
                        ProductId = id,
                        UserId = userId
                    };
                    db.Wishlists.Add(wishlist);
                    db.SaveChanges();
                }
                return RedirectToAction("Wishlist");
            }
            return RedirectToAction("Login", "User");
        }

        // Show all wishlist items
        public ActionResult Wishlist()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                int userId = Convert.ToInt32(userInCookie["idUser"]);
                var wishItems = db.Wishlists.Include("Product").Where(w => w.UserId == userId).ToList();
                return View(wishItems);
            }
            return RedirectToAction("Login", "User");
        }

        // Move from wishlist to cart
        public ActionResult MoveToCart(int id)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                int userId = Convert.ToInt32(userInCookie["idUser"]);
                var wishlistItem = db.Wishlists.Include("Product")
                    .FirstOrDefault(w => w.Id == id && w.UserId == userId);

                if (wishlistItem != null)
                {
                    // Create cart item
                    Cart cart = new Cart
                    {
                        productId = wishlistItem.Product.id,
                        productName = wishlistItem.Product.ProductName,
                        productPic = wishlistItem.Product.ProductPicture,
                        price = wishlistItem.Product.ProductPrice,
                        qty = 1,
                        bill = wishlistItem.Product.ProductPrice
                    };

                    List<Cart> li2 = TempData["cart"] as List<Cart> ?? new List<Cart>();
                    li2.Add(cart);
                    TempData["Success"] = "Added to Card successfully!";
                    TempData["cart"] = li2;
                    TempData.Keep();

                    // remove from wishlist
                    db.Wishlists.Remove(wishlistItem);
                    db.SaveChanges();
                }
                return RedirectToAction("Wishlist");
            }
            return RedirectToAction("Login", "User");
        }

        
    
        public ActionResult MyOrders()
        {
            // Get current logged-in user info from cookie
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie == null)
            {
                // If not logged in, redirect to login page
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(userInCookie["idUser"]);

            // Get only confirmed orders for this user
            var orders = db.orders
                .Include("prodcts")
                .Include("invoices")
                .Where(o => o.invoices.FKUserID == userId)
                .OrderByDescending(o => o.Order_Date)
                .ToList();

            return View(orders);
        }

        public ActionResult Search(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction("Index");
            }

              var products = db.Products
                     .Where(p => p.ProductName.Contains(q))
                     .ToList();
            return View(products);
        }
    }
}