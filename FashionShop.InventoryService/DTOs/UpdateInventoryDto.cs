namespace FashionShop.InventoryService.DTOs
{
    public class UpdateInventoryDto
    {
        public Guid ProductId { get; set; }
        public int QuantityChange { get; set; }

    }
}
