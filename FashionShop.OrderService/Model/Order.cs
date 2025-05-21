namespace FashionShop.OrderService.Model
{
    public class Order : BaseEntity
    {

        public Guid? UserId { get; set; }

        public Guid? PaymentId { get; set; }
        public string? OrderStatus { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        public PaymentDetail? PaymentDetail { get; set; }
    }
}
