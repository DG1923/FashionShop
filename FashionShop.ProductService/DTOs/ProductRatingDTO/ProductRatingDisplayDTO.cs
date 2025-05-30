namespace FashionShop.ProductService.DTOs.ProductRatingDTO
{
    public class ProductRatingDisplayDTO
    {
        // Changed TotalRating to a property with a getter to avoid referencing non-static fields in a field initializer.  
        public int TotalRating
        {
            get
            {
                return ProductRatings.Any() ? ProductRatings.Count() : 0;
            }
        }

        public decimal AverangeRating
        {
            get
            {
                if (ProductRatings.Any())
                {
                    return Math.Round(ProductRatings.Average(r => r.Rating ?? 0), 2);
                }
                return 0;
            }
        }

        public IEnumerable<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();
    }
}
