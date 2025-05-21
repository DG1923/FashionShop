using FashionShop.OrderService.Model;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Repo.Interface
{
    public interface IOrderRepo : IGenericRepo<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<bool> UpdateOrderStatusAsync(Guid id, string status);
    }

}
