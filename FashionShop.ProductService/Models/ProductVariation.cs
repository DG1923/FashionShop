using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class ProductVariation:BaseEntity
    {
        [Required]
        public string Color { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]  
        public decimal AdditionalPrice { get; set; }//additional price for the variation from base cost
        [Required]
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        //configure the relationship with Product   
        public Product? Product { get; set; }
        [Required]
        public Guid ProductId { get; set; }
    }
}
