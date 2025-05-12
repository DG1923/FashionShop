using System.ComponentModel.DataAnnotations;

namespace FashionShop.InventoryService.DTOs
{
    public class AddInventoryDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
