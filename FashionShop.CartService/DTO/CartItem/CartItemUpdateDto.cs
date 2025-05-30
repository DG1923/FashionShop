using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.DTO.CartItem
{
    public class CartItemUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid ProductColorId { get; set; }

        [Required]
        [StringLength(100)]
        public string ColorName { get; set; }

        public string? ColorCode { get; set; }

        [Required]
        public Guid ProductVariationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Size { get; set; }

        public Guid InventoryId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }


    }
}
