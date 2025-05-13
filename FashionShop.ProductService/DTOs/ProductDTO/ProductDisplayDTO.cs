using FashionShop.ProductService.DTOs.DiscountDTO;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.ProductDTO
{
    public class ProductDisplayDTO
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        //[MinLength(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal BasePrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal? DiscountedPrice { get; set; }

        public string? MainImageUrl { get; set; }
        public DiscountDisplayDTO? discountDisplayDTO { get; set; }
    }
}
