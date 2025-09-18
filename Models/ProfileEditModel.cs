using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodWeb.Models
{
    public class ProfileEditModel
    {
        public int userid { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}