using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FoodWeb.Models;
using FoodWeb.Services;

namespace FoodWeb.Controllers
{
    [AllowAnonymous]
    public class AchievementsController : Controller
    {
        private readonly AppFoodDbContext _context;
        private readonly IChallengeProgressService _progressService;

        public AchievementsController()
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

        // GET: Achievements
        public async Task<ActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var achievements = await _context.Achievements
                .Where(a => a.IsActive)
                .OrderBy(a => a.Title)
                .ToListAsync();

            var userAchievements = await _context.UserAchievements
                .Where(ua => ua.UserId == userId.Value)
                .ToDictionaryAsync(ua => ua.AchievementId, ua => ua);

            ViewBag.UserAchievements = userAchievements;
            return View(achievements);
        }

        // GET: Achievements/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(m => m.Id == id);

            if (achievement == null)
            {
                return HttpNotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userAchievement = await _context.UserAchievements
                .FirstOrDefaultAsync(ua => ua.UserId == userId.Value && ua.AchievementId == id);

            ViewBag.UserAchievement = userAchievement;
            return View(achievement);
        }

        // GET: Achievements/MyAchievements
        public async Task<ActionResult> MyAchievements()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userAchievements = await _progressService.GetUserAchievementsAsync(userId.Value);

            // Get all achievements to show locked ones too
            var allAchievements = await _context.Achievements
                .Where(a => a.IsActive)
                .ToListAsync();

            var achievementStatus = allAchievements.Select(a => new AchievementStatusViewModel
            {
                Achievement = a,
                UserAchievement = userAchievements.FirstOrDefault(ua => ua.AchievementId == a.Id),
                ProgressPercentage = CalculateProgressPercentage(a, userAchievements.FirstOrDefault(ua => ua.AchievementId == a.Id))
            }).ToList();

            return View(achievementStatus);
        }

        // GET: Achievements/Leaderboard
        // GET: Achievements/Leaderboard
        // GET: Achievements/Leaderboard
        public async Task<ActionResult> Leaderboard()
        {
            var topUsers = await _context.UserAchievements
                .Where(ua => ua.IsUnlocked)
                .Include(ua => ua.Achievement)
                .GroupBy(ua => ua.UserId)
                .Select(g => new LeaderboardViewModel
                {
                    UserId = g.Key,
                    AchievementCount = g.Count(),
                    TotalPoints = g.Sum(ua => ua.Achievement.RewardPoints)
                })
                .OrderByDescending(x => x.AchievementCount)
                .ThenByDescending(x => x.TotalPoints)
                .Take(10)
                .ToListAsync();

            // Get usernames for display - Use SignupLogin instead of UserAccount
            var userIds = topUsers.Select(u => u.UserId).ToList();

            var users = await _context.SignupLogin
                .Where(u => userIds.Contains(u.userid))  // Note: userid instead of Id
                .ToDictionaryAsync(u => u.userid, u => u.Name);  // Note: Name instead of UserName

            foreach (var user in topUsers)
            {
                user.Username = users.ContainsKey(user.UserId) ? users[user.UserId] : "User#" + user.UserId;
            }

            return View(topUsers);
        }

        // Helper methods
        private double CalculateProgressPercentage(Achievement achievement, UserAchievement userAchievement)
        {
            if (userAchievement == null) return 0;
            if (userAchievement.IsUnlocked) return 100;

            switch (achievement.AchievementType)
            {
                case "PURCHASE":
                    if (achievement.RequiredQuantity.HasValue && achievement.RequiredQuantity > 0)
                    {
                        return (double)userAchievement.Progress / achievement.RequiredQuantity.Value * 100;
                    }
                    break;
                case "SPENDING_AMOUNT":
                    if (achievement.RequiredAmount.HasValue && achievement.RequiredAmount > 0)
                    {
                        return (double)userAchievement.Progress / (double)achievement.RequiredAmount.Value * 100;
                    }
                    break;
                case "CHALLENGE_COMPLETION":
                    if (achievement.RequiredChallenges.HasValue && achievement.RequiredChallenges > 0)
                    {
                        return (double)userAchievement.Progress / achievement.RequiredChallenges.Value * 100;
                    }
                    break;
                case "CHALLENGE_JOIN":
                    if (achievement.RequiredQuantity.HasValue && achievement.RequiredQuantity > 0)
                    {
                        return (double)userAchievement.Progress / achievement.RequiredQuantity.Value * 100;
                    }
                    break;
            }

            return 0;
        }
    }

    // View Models
    public class AchievementStatusViewModel
    {
        public Achievement Achievement { get; set; }
        public UserAchievement UserAchievement { get; set; }
        public double ProgressPercentage { get; set; }
    }

    public class LeaderboardViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int AchievementCount { get; set; }
        public decimal TotalPoints { get; set; }
    }
}