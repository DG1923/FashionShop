
using FashionShop.InventoryService.DTOs;
using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Service.Interface;
using System.Text.Json;

namespace FashionShop.ProductService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermindEvent(message);
            switch (eventType)
            {
                case EventType.InventoryPublished:
                    Console.WriteLine("--> Processing Inventory Published event");
                    UpdataQuantity(message);
                    break;
                case EventType.OrderCreated:
                    Console.WriteLine("--> Processing Inventory when order created !");
                    break;
                default:
                    Console.WriteLine("--> Event type not recognized");
                    break;
            }
        }

        private async Task UpdataQuantity(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IProductVariationService>();
                PublishInventoryDto publishInventoryDto = JsonSerializer.Deserialize<PublishInventoryDto>(message);
                Guid productVariationId = publishInventoryDto.ProductId;
                int quantity = publishInventoryDto.Quantity;
                try
                {
                    var productVariationExit = await repo.GetByIdAsync(productVariationId);
                    if (productVariationExit == null)
                    {
                        Console.WriteLine($"--> Product variation with ID {productVariationId} does not exist");
                        return;
                    }
                    Console.WriteLine($"--> Updating product variation with ID {productVariationId} to quantity {quantity}");
                    await repo.UpdateQuantity(productVariationId, quantity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error updating product quantity: {ex.Message}");
                }
            }
        }

        private EventType DetermindEvent(string message)
        {
            Console.WriteLine("--> Determining event type...");
            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(message);
            switch (eventType.Event)
            {
                case "InventoryPublished":
                    Console.WriteLine("--> Inventory published event detected");
                    // Handle the event
                    return EventType.InventoryPublished;
                case "OrderCreated":
                    Console.WriteLine("--> Order created event detected");
                    // Handle the event
                    return EventType.OrderCreated;

                default:
                    Console.WriteLine("--> Event type not recognized");
                    return EventType.Undetermined;
            }

        }
        enum EventType
        {
            InventoryPublished,
            OrderCreated,
            Undetermined
        }
    }
}
