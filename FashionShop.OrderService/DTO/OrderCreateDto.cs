using System.ComponentModel.DataAnnotations;

namespace FashionShop.OrderService.DTO
{
    public class OrderCreateDto
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        [Phone]
        [Required]
        public string ContactNumber { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; } = new();
        public PaymentDetailCreateDto? PaymentDetail { get; set; }
    }
}
