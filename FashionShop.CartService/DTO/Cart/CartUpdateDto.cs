using FashionShop.CartService.DTO.CartItem;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.DTO.Cart
{
    public class CartUpdateDto
    {
        [Required]
        public Guid CartId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public List<CartItemUpdateDto> Items { get; set; } = new List<CartItemUpdateDto>();
    }
}
