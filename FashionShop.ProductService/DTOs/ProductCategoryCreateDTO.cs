using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs
{
    public class ProductCategoryCreateDTO
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string? ImageUrl { get; set; }
    }
}
