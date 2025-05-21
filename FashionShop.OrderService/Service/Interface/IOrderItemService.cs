using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.Service.Interface
{
    public interface IOrderItemService : IBaseService<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
        Task<bool> UpdateQuantityAsync(Guid orderItemId, int quantity);
        Task<bool> DeleteOrderItemAsync(Guid orderItemId);
    }
}
