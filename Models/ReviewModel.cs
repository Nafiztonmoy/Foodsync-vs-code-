using System;
using System.ComponentModel.DataAnnotations;

namespace FoodWeb.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public string ImagePath { get; set; } // store image path

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
