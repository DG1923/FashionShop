using System.ComponentModel.DataAnnotations;

namespace FashionShop.CartService.DTO.CartItem
{
    public class CartItemDisplayDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid CartId { get; set; }
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(500)]
        public string ProductName { get; set; }

        public Guid? DiscountId { get; set; }
        [Required]
        public decimal BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }

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

        [Url]
        public string? ImageUrl { get; set; }

        // Computed property for total price
        public decimal TotalPrice
        {
            get
            {
                if (DiscountPercent.HasValue && DiscountPercent.Value > 0)
                {
                    return BasePrice * Quantity * (1 - DiscountPercent.Value / 100);
                }
                return BasePrice * Quantity;
            }
        }
    }
}
