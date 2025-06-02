using System.ComponentModel.DataAnnotations;

namespace FashionShop.OrderService.DTO
{
    public class ReturnReviewDto
    {
        [Required]
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
