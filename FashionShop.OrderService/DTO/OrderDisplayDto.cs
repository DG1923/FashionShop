using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.DTO
{
    public class OrderDisplayDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Address { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public string? FullName { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<OrderItemDisplayDto>? OrderItems { get; set; }
        public PaymentDetailDisplayDto? PaymentDetail { get; set; }
    }
    public class OrderItemDisplayDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public Guid ProductColorId { get; set; }
    }

    public class PaymentDetailDisplayDto
    {
        public Guid Id { get; set; }
        public string PaymentStatus { get; set; }
    }
}
