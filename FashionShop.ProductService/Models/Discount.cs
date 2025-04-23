using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.Models
{
    public class Discount:BaseEntity
    {

        [MaxLength(2000)]   
        public string Description { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal DiscountPercent { get; set; }
        [Required]
        public bool IsActive { get; set; }



        public ICollection<Product>? Products { get; set; }

    }
}
