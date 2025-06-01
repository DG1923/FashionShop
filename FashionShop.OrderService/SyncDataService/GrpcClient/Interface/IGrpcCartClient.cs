using FashionShop.OrderService.Model;
using FashionShop.OrderService.Proto;

namespace FashionShop.OrderService.SyncDataService.GrpcClient.Interface
{
    public interface IGrpcCartClient
    {
        Task<IEnumerable<OrderItem>> GetCartItemsAsync(Guid cartId);
        Task<bool> ClearCartAsync(ClearListCartItemRequest listCartItemId);
    }
}
