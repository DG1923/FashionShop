namespace FashionShop.ProductService.DTOs
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
