using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.Models
{
    public class Cart : BaseEntity
    {

        [Required]
        public Guid UserId { get; set; }
        public List<CartItem>? Items { get; set; } = new List<CartItem>();
    }
}
