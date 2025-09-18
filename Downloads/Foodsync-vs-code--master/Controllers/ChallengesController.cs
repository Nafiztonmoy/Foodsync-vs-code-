
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FoodWeb.Models;
using FoodWeb.Services;

namespace FoodWeb.Controllers
{
    [AllowAnonymous]
    public class ChallengesController : Controller
    {
        private readonly AppFoodDbContext _context;
        private readonly IChallengeProgressService _progressService;

        public ChallengesController()
        {
            _context = new AppFoodDbContext();
            _progressService = new ChallengeProgressService(_context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        // Helper method to get user ID from cookies
        private int? GetUserId()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null && !string.IsNullOrEmpty(userInCookie["idUser"]))
            {
                if (int.TryParse(userInCookie["idUser"], out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        // GET: Challenges
        public async Task<ActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var challenges = await _context.Challenges
                .Where(c => c.IsActive && c.EndDate > DateTime.Now)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var userChallenges = await _progressService.GetUserChallengesAsync(userId.Value);

            ViewBag.UserChallenges = userChallenges;
            return View(challenges);
        }

        // GET: Challenges/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return HttpNotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userChallenge = await _context.UserChallenges
                .FirstOrDefaultAsync(uc => uc.UserId == userId.Value && uc.ChallengeId == id);

            ViewBag.UserChallenge = userChallenge;
            return View(challenge);
        }

        // POST: Challenges/Join/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Join(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var result = await _progressService.JoinChallengeAsync(userId.Value, id);

            if (result)
            {
                TempData["SuccessMessage"] = "You've successfully joined the challenge!";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to join the challenge. You may have already joined or the challenge is no longer available.";
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: Challenges/QuickBuy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuickBuy(Models.QuickBuyDto model) // Explicitly use Models.QuickBuyDto
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            model.UserId = userId.Value;
            var result = await _progressService.QuickBuyFromChallengeAsync(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Purchase recorded! Your challenge progress has been updated.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to process your purchase. Please try again.";
            }

            return RedirectToAction("Details", new { id = model.ChallengeId });
        }

        // GET: Challenges/MyProgress
        public async Task<ActionResult> MyProgress()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userChallenges = await _progressService.GetUserChallengesAsync(userId.Value);
            return View(userChallenges);
        }
    }
}
