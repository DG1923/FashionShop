namespace FashionShop.InventoryService.DTOs
{
    public class InventoryDisplayDto
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
