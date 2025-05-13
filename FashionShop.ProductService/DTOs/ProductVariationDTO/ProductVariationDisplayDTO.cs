namespace FashionShop.ProductService.DTOs.ProductVariationDTO
{
    public class ProductVariationDisplayDTO
    {

        public Guid Id { get; set; }
        public string Size { get; set; }
        public string? Description { get; set; }

        public int Quantity { get; set; }
        public string? ImageUrlVariation { get; set; }
    }
}
