using System.ComponentModel.DataAnnotations;

namespace FashionShop.OrderService.Model
{
    public class BaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
