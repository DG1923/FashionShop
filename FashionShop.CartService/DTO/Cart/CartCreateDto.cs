using FashionShop.CartService.DTO.CartItem;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.DTO.Cart
{
    public class CartCreateDto
    {
        [Required]
        public Guid UserId { get; set; }


        public List<CartItemCreateDto>? Items { get; set; } = new List<CartItemCreateDto>();
    }
}
