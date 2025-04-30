using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class Product:BaseEntity
    {
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
        
        [MaxLength(100)]
        public string? SKU { get; set; }
        public int? TotalQuantity { get; set; }
        public string? MainImageUrl { get; set; }




        //configure the relationship with ProductVariation
        public IEnumerable<ProductColor>? ProductColors { get; set; }

        //configure the relationship with Discount
        public Guid? DiscountId { get; set; }
        public Discount? Discount { get; set; }

        //configure the relationship with ProductCategory
        public Guid? ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        



    }
}
