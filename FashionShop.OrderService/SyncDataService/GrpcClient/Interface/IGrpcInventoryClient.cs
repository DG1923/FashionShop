using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.SyncDataService.GrpcClient.Interface
{
    public interface IGrpcInventoryClient
    {
        Task<(bool success, int quantity)> GetInventoryQuantityAsync(Guid productId);
        Task<bool> UpdateInventoryQuantitiesAsync(List<OrderItem> items);
    }
}
