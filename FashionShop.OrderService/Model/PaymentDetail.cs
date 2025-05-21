namespace FashionShop.OrderService.Model
{
    public class PaymentDetail : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }

}
