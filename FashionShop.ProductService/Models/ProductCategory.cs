using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class ProductCategory: BaseEntity    
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(2000)]
        
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<ProductCategory>? SubCategories { get; set; }
    }
}
