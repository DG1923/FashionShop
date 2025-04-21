using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class Product:BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        [MinLength(0)]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; }

        //configure the relationship with ProductVariation
        public ICollection<ProductVariation>? Variations { get; set; }

        //configure the relationship with Discount
        public Guid DiscountId { get; set; }
        public Discount? Discount { get; set; }

        //configure the relationship with ProductCategory
        public Guid ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        



    }
}
