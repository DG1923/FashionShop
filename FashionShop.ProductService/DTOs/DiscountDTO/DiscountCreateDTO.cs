using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.DiscountDTO
{
    public class DiscountCreateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal DiscountPercent { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
