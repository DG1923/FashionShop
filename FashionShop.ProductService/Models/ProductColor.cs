using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class ProductColor:BaseEntity
    {
        [Required]
        public string ColorName { get; set; }
        public string? ColorCode { get; set; }  
        public string? ImageUrlColor { get; set; }
        
        public int? TotalQuantityColor { get; set; }



        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public ICollection<ProductVariation>? ProductVariations { get; set; }

    }
}
