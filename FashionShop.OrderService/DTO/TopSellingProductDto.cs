namespace FashionShop.OrderService.DTO
{
    public class TopSellingProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
