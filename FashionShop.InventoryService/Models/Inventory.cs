using System.ComponentModel.DataAnnotations;

namespace FashionShop.InventoryService.Models
{
    public class Inventory:BaseEntity
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public Guid ProductId { get; set; }


    }
}
