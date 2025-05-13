namespace FashionShop.ProductService.DTOs.ProductVariationDTO
{
    public class ProductVariationCreateDTO
    {
        public string Id { get; set; }
        public string Size { get; set; }
        public string? Description { get; set; }

        public int Quantity { get; set; }
        public string? ImageUrlVariation { get; set; }
    }
}
