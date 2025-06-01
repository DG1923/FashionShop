namespace FashionShop.OrderService.DTO
{
    public class PaymentDetailCreateDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
    }
}
