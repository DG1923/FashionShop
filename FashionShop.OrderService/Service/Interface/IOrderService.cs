using FashionShop.OrderService.DTO;
using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.Service.Interface
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<decimal> CalculateOrderTotalAsync(Guid orderId);
        Task<bool> CreateOrderAsync(OrderCreateDto order, Guid cartId);
    }
}
