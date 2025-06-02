namespace FashionShop.OrderService.Model
{
    public class Order : BaseEntity
    {

        public Guid? UserId { get; set; }
        public string Address { get; set; }
        public Guid? PaymentId { get; set; }
        public OrderStatus Status { get; set; }
        public string OrderStatus { get; set; }

        public string? ReturnReason { get; set; }
        public string? ReturnRejectionReason { get; set; }
        public DateTime? ReturnRequestDate { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }

        public PaymentDetail? PaymentDetail { get; set; }
    }
}
