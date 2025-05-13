using FashionShop.ProductService.DTOs.ProductVariationDTO;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs.ProductColorDTO
{
    public class ProductColorCreateDTO
    {
        public string Id { get; set; }
        [Required]
        public string ColorName { get; set; }
        public string? ColorCode { get; set; }
        public string? ImageUrlColor { get; set; }
        public List<ProductVariationCreateDTO>? productVariationDisplayDTOs { get; set; }
    }
}
