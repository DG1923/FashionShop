using System.ComponentModel.DataAnnotations;

namespace FashionShop.OrderService.DTO
{
    public class ReturnRequestDto
    {
        [Required]
        public string Reason { get; set; }
    }
}
