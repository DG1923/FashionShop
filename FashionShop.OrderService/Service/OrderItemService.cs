using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.OrderService.Service.Interface;

namespace FashionShop.OrderService.Service
{
    public class OrderItemService : BaseService<OrderItem>, IOrderItemService
    {
        private readonly IOrderItemRepo _orderItemRepo;

        public OrderItemService(IOrderItemRepo orderItemRepo) : base(orderItemRepo)
        {
            _orderItemRepo = orderItemRepo;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            return await _orderItemRepo.GetOrderItemsByOrderIdAsync(orderId);
        }

        public async Task<bool> UpdateQuantityAsync(Guid orderItemId, int quantity)
        {
            return await _orderItemRepo.UpdateOrderItemQuantityAsync(orderItemId, quantity);
        }

        public async Task<bool> DeleteOrderItemAsync(Guid orderItemId)
        {
            return await _orderItemRepo.DeleteOrderItemAsync(orderItemId);
        }
    }
}
