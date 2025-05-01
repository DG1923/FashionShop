using System.ComponentModel.DataAnnotations;

namespace FashionShop.ProductService.DTOs
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
