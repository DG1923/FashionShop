using FashionShop.InventoryService.DTOs;

namespace FashionShop.InventoryService.AsynDataService
{
    public interface IMessageBus
    {
        void PublishUpdateQuantity(PublishInventoryDto publishInventoryDto);
    }
}
