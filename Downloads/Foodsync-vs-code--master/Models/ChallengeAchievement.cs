// First, let's fix the models to use proper casing

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWeb.Models
{
    public class Challenge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string RequiredProduct { get; set; }

        [Required]
        public int RequiredQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RewardPoints { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
    }

    public class UserChallenge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChallengeId { get; set; }

        public int CurrentProgress { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("ChallengeId")]
        public virtual Challenge Challenge { get; set; }
    }

    public class Achievement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string AchievementType { get; set; } // "PURCHASE", "CHALLENGE_COMPLETION", "SPENDING_AMOUNT"

        public string RequiredProduct { get; set; }
        public int? RequiredQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RequiredAmount { get; set; }

        public int? RequiredChallenges { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RewardPoints { get; set; }

        public string BadgeIcon { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }

    public class UserAchievement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AchievementId { get; set; }

        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }
        public int Progress { get; set; } = 0;

        // Navigation properties
        [ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
    }

    // DTO for challenge progress updates
    public class ChallengeProgressDto
    {
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    // DTO for quick buy from challenge
    public class QuickBuyDto
    {
        public int ChallengeId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
