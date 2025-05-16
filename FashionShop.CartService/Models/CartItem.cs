using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.Models
{
    public class CartItem : BaseEntity
    {

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Product name cannot exceed 500 characters.")]
        public string ProductName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal zero.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string? ImageUrl { get; set; }
    }
}