using FashionShop.OrderService.DTO;
using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.Service.Interface
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        //Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<decimal> CalculateOrderTotalAsync(Guid orderId);
        Task<bool> CreateOrderAsync(OrderCreateDto order, Guid cartId);
        Task<bool> RequestReturnAsync(Guid orderId, ReturnRequestDto request);
        Task<bool> ProcessReturnRequestAsync(Guid orderId, ReturnReviewDto review);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<Order> GetOrderStatusWithHistoryAsync(Guid orderId);
    }
}

