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
        Task<IEnumerable<OrderDisplayDto>> GetOrdersByStatusAsync(OrderStatus status, Guid userId);
        Task<PagedList<OrderDisplayDto>> GetAllOrdersPaged(int pageNumber, int pageSize = 16);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IDictionary<string, decimal>> GetMonthlyRevenueAsync(int year);
        Task<IDictionary<int, decimal>> GetYearlyRevenueAsync();
        Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(int limit = 10);
    }
}

