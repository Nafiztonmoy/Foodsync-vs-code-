using FoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

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

        // GET: User Profile
        public new ActionResult Profile()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                int userId = Convert.ToInt32(userInCookie.Values["idUser"]);
                var user = db.SignupLogin.Find(userId);
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Edit Profile
        [HttpGet]
        public ActionResult EditProfile()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                int userId = Convert.ToInt32(userInCookie.Values["idUser"]);
                var user = db.SignupLogin.Find(userId);
                if (user != null)
                {
                    // Create a view model to avoid showing password fields
                    var profileModel = new ProfileEditModel
                    {
                        userid = user.userid,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Address = user.Address
                    };
                    return View(profileModel);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(ProfileEditModel model)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        int userId = Convert.ToInt32(userInCookie.Values["idUser"]);
                        var user = db.SignupLogin.Find(userId);

                        if (user != null)
                        {
                            // Check if email is being changed and if new email already exists
                            if (user.Email != model.Email)
                            {
                                var emailExists = db.SignupLogin.Any(x => x.Email == model.Email && x.userid != userId);
                                if (emailExists)
                                {
                                    ModelState.AddModelError("Email", "This email is already registered by another user.");
                                    return View(model);
                                }
                            }

                            // Update user information
                            user.Name = model.Name;
                            user.Email = model.Email;
                            user.Phone = model.Phone;
                            user.Address = model.Address;

                            db.SaveChanges();

                            // Update cookie with new information
                            HttpCookie updatedCookie = new HttpCookie("UserInfo");
                            updatedCookie.Values["idUser"] = Convert.ToString(user.userid);
                            updatedCookie.Values["FullName"] = user.Name;
                            updatedCookie.Values["Email"] = user.Email;
                            updatedCookie.Expires = DateTime.Now.AddMonths(1);
                            Response.Cookies.Add(updatedCookie);

                            TempData["Success"] = "Profile updated successfully!";
                            return RedirectToAction("Profile");
                        }
                        else
                        {
                            return RedirectToAction("Login");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred while updating your profile: {ex.Message}";
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Change Password
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Change Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Clear any previous model state errors
                ModelState.Clear();
                
                // Re-validate the model to ensure fresh validation
                TryValidateModel(model);
                
                if (ModelState.IsValid)
                {
                    int userId = Convert.ToInt32(userInCookie.Values["idUser"]);
                    var user = db.SignupLogin.Find(userId);
                    
                    if (user == null)
                    {
                        TempData["Error"] = "User not found. Please log in again.";
                        return RedirectToAction("Login");
                    }
                    
                    // Verify current password
                    if (user.Password != model.CurrentPassword)
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        TempData["Error"] = "Current password is incorrect.";
                        return View(model);
                    }
                    
                    // Additional password validation
                    if (!IsValidPassword(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "Password does not meet security requirements.");
                        TempData["Error"] = "Password must be at least 6 characters and contain uppercase, lowercase, number, and special character.";
                        return View(model);
                    }
                    
                    // Update password with transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            user.Password = model.NewPassword;
                            user.ConfirmPassword = model.NewPassword;
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();
                            
                            TempData["Success"] = "Password changed successfully!";
                            return RedirectToAction("Profile");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            TempData["Error"] = "Database error occurred. Please try again.";
                            System.Diagnostics.Debug.WriteLine($"Password change error: {ex.Message}");
                            return View(model);
                        }
                    }
                }
                else
                {
                    // Collect and display validation errors
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    TempData["Error"] = string.Join("<br/>", errors);
                    System.Diagnostics.Debug.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ChangePassword exception: {ex.Message}");
            }
            
            return View(model);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;
                
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => "@$!%*?&".Contains(c));
            
            return hasLower && hasUpper && hasDigit && hasSpecial;
        }

        // GET: Forgot Password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.SignupLogin.FirstOrDefault(u => u.Email == model.Email);

                    if (user == null)
                    {
                        TempData["Error"] = "No account found with this email address.";
                        return View(model);
                    }

                    // Generate password reset token
                    var emailService = new EmailService();
                    var token = emailService.GeneratePasswordResetToken();
                    var expiryTime = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

                    // Save token to database
                    var resetToken = new PasswordResetToken
                    {
                        UserId = user.userid,
                        Token = token,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = expiryTime,
                        IsUsed = false
                    };

                    db.PasswordResetTokens.Add(resetToken);
                    db.SaveChanges();

                    // Generate reset link
                    var resetLink = Url.Action("ResetPassword", "User", new { token = token, email = model.Email }, Request.Url.Scheme);

                    // Send email
                    var emailSent = await emailService.SendPasswordResetEmailAsync(user.Email, resetLink, user.Name);

                    if (emailSent)
                    {
                        TempData["Success"] = "Password reset instructions have been sent to your email address. Please check your inbox and follow the instructions to reset your password.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Error"] = "There was an error sending the reset email. Please try again later.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while processing your request. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ForgotPassword Error: {ex.Message}");
            }

            return View(model);
        }

        // GET: Reset Password
        [HttpGet]
        public ActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            // Verify token
            var resetToken = db.PasswordResetTokens.FirstOrDefault(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
            if (resetToken == null)
            {
                TempData["Error"] = "This password reset link has expired or is invalid. Please request a new password reset.";
                return RedirectToAction("ForgotPassword");
            }

            // Verify email matches
            var user = db.SignupLogin.Find(resetToken.UserId);
            if (user == null || user.Email != email)
            {
                TempData["Error"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        // POST: Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verify token
                    var resetToken = db.PasswordResetTokens.FirstOrDefault(t => t.Token == model.Token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
                    if (resetToken == null)
                    {
                        TempData["Error"] = "This password reset link has expired or is invalid.";
                        return RedirectToAction("ForgotPassword");
                    }

                    // Get user
                    var user = db.SignupLogin.Find(resetToken.UserId);
                    if (user == null || user.Email != model.Email)
                    {
                        TempData["Error"] = "Invalid password reset request.";
                        return RedirectToAction("Login");
                    }

                    // Update password with transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            user.Password = model.NewPassword;
                            user.ConfirmPassword = model.NewPassword;
                            resetToken.IsUsed = true;

                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.Entry(resetToken).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();

                            TempData["Success"] = "Your password has been successfully reset! You can now log in with your new password.";
                            return RedirectToAction("Login");
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while resetting your password. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ResetPassword Error: {ex.Message}");
            }

            return View(model);
        }

        // GET: Google Login
        public ActionResult GoogleLogin()
        {
            try
            {
                var googleService = new GoogleOAuthService();
                var authUrl = googleService.GetAuthorizationUrl();
                return Redirect(authUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Google authentication service is currently unavailable. Please try logging in with your email and password.";
                System.Diagnostics.Debug.WriteLine($"GoogleLogin Error: {ex.Message}");
                return RedirectToAction("Login");
            }
        }

        // GET: Google Callback
        public async Task<ActionResult> GoogleCallback(string code, string error, string state)
        {
            if (!string.IsNullOrEmpty(error))
            {
                TempData["Error"] = $"Google authentication failed: {error}. Please try again or use email/password login.";
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(code))
            {
                TempData["Error"] = "Invalid Google authentication response.";
                return RedirectToAction("Login");
            }

            try
            {
                var googleService = new GoogleOAuthService();
                var userInfo = await googleService.GetUserInfoAsync(code);

                if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
                {
                    TempData["Error"] = "Failed to retrieve user information from Google. Please try again or use email/password login.";
                    return RedirectToAction("Login");
                }

                // Check if user exists
                var existingUser = db.SignupLogin.FirstOrDefault(u => u.Email == userInfo.Email);

                if (existingUser == null)
                {
                    // Create new user account
                    var randomPassword = googleService.GenerateRandomPassword();
                    var newUser = new SignupLogin
                    {
                        Name = userInfo.Name ?? "Google User",
                        Email = userInfo.Email,
                        Password = randomPassword,
                        ConfirmPassword = randomPassword
                    };

                    try
                    {
                        db.SignupLogin.Add(newUser);
                        db.SaveChanges();
                        existingUser = newUser;

                        TempData["Success"] = $"Welcome to Food Sync, {existingUser.Name}! Your account has been created successfully using Google authentication.";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"User creation error: {ex.Message}");
                        TempData["Error"] = "Failed to create user account. Please try again or contact support.";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["Success"] = $"Welcome back, {existingUser.Name}! You have successfully logged in with Google.";
                }

                // Create authentication cookie
                CreateUserCookie(existingUser);
                Session["uid"] = existingUser.userid;

                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred during Google authentication. Please try again or use email/password login.";
                System.Diagnostics.Debug.WriteLine($"GoogleCallback Error: {ex.Message}");
                return RedirectToAction("Login");
            }
        }

        private void CreateUserCookie(SignupLogin user)
        {
            HttpCookie cookie = new HttpCookie("UserInfo");
            cookie.Values["idUser"] = Convert.ToString(user.userid);
            cookie.Values["FullName"] = Convert.ToString(user.Name);
            cookie.Values["Email"] = Convert.ToString(user.Email);
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(cookie);
        }
    }
}