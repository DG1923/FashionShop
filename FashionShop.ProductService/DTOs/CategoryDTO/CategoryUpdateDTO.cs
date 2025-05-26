using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.CategoryDTO
{
    public class CategoryUpdateDTO
    {

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(2000)]

        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public IEnumerable<ProductCategoryDisplayDTO>? SubCategory { get; set; }

    }
}
