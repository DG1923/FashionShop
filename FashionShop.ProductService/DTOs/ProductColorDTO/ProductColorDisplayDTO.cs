using FashionShop.ProductService.DTOs.ProductVariationDTO;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.ProductColorDTO
{
    public class ProductColorDisplayDTO
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ColorName { get; set; }
        public string? ColorCode { get; set; }
        public string? ImageUrlColor { get; set; }
        public List<ProductVariationDisplayDTO>? productVariationDisplayDTOs { get; set; }
    }
}
