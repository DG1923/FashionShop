using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class ProductVariation:BaseEntity
    {

        [Required]
        public string Size { get; set; }
        public string? Description { get; set; }
        
        public int Quantity { get; set; }
        public string? ImageUrlVariation { get; set; }
        //configure the relationship with Product   
        public ProductColor? ProductColor { get; set; }
        [Required]
        public Guid ProductColorId { get; set; }
    }
}
