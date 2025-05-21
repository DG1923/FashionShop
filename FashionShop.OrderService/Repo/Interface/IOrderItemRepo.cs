using FashionShop.OrderService.Model;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Repo.Interface
{
    public interface IOrderItemRepo : IGenericRepo<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
        Task<bool> UpdateOrderItemQuantityAsync(Guid orderItemId, int quantity);
        Task<bool> DeleteOrderItemAsync(Guid orderItemId);
    }

}
