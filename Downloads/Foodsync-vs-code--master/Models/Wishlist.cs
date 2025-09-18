using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWeb.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        // link to Product
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; }

        // link to User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual SignupLogin User { get; set; }
    }
}
