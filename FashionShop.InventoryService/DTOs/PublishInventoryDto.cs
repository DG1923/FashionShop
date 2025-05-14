namespace FashionShop.InventoryService.DTOs
{
    public class PublishInventoryDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Event { get; set; }
    }
}
