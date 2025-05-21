namespace FashionShop.OrderService.Model
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        // Thông tin về màu sắc
        public Guid ProductColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }

        // Thông tin về size và số lượng
        public Guid ProductVariationId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }

        // Lưu URL hình ảnh
        public string ImageUrl { get; set; }
    }
}
