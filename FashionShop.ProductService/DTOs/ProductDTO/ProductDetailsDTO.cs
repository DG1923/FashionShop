using FashionShop.ProductService.DTOs.CategoryDTO;
using FashionShop.ProductService.DTOs.DiscountDTO;
using FashionShop.ProductService.DTOs.ProductColorDTO;
using FashionShop.ProductService.Models;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.ProductDTO
{
    public class ProductDetailsDTO
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Required]
        //[MinLength(0)] it is used by type of string
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal BasePrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal? DiscountedPrice { get; set; }
        public string? MainImageUrl { get; set; }


        public DiscountDisplayDTO? discountDisplayDTO { get; set; }

        public IEnumerable<ProductColorDisplayDTO>? productColorsDisplayDTO { get; set; }
        public ProductCategoryDisplayDTO? productCategoryDisplayDTO { get; set; }


    }
}
