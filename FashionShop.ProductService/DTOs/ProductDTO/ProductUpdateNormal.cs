using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.ProductDTO
{
    public class ProductUpdateNormal
    {
      
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        //[MinLength(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
