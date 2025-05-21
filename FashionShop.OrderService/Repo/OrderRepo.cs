using FashionShop.OrderService.Data;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.OrderService.Repo
{
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {
        private readonly OrderDbContext _context;
        private readonly IRedisCacheManager<Order> _redis;
        private readonly DbSet<Order> _dbSetOrder;
        private readonly string _cachePrefix;

        public OrderRepo(OrderDbContext context, IRedisCacheManager<Order> redisCacheManager) : base(context, redisCacheManager)
        {
            _context = context;
            _redis = redisCacheManager;
            _dbSetOrder = _context.Set<Order>();
            _cachePrefix = typeof(Order).Name.ToLower();
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            string cacheKey = $"{_cachePrefix}_By_userId_{userId}";

            // Specify the type argument explicitly to fix CS0411
            var cachedOrders = await _redis.GetAsync<IEnumerable<Order>>(cacheKey);
            if (cachedOrders != null)
            {
                return cachedOrders;
            }

            List<Order> orders = await _dbSetOrder.Where(o => o.UserId == userId).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return null;
            }

            // Cache the orders for future use
            try
            {
                await _redis.SetAsync(cacheKey, orders);
                Console.WriteLine($"[OrderRepo] set cache for key: {cacheKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderRepo] Cache operation failed: {ex.Message}");
            }
            return orders;
        }
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            string cacheKey = $"{_cachePrefix}_By_status_{status}";
            var cachedOrders = await _redis.GetAsync<IEnumerable<Order>>(cacheKey);
            if (cachedOrders != null)
            {
                return cachedOrders;
            }
            IEnumerable<Order> orders = await _dbSetOrder.Where(o => o.OrderStatus == status).ToListAsync();
            if (orders == null || !orders.Any())
            {
                return null;
            }
            try
            {
                await _redis.SetAsync(cacheKey, orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderRepo] Cache operation failed: {ex.Message}");
            }
            return orders;
        }
        public async Task<bool> UpdateOrderStatusAsync(Guid id, string status)
        {


            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;
            order.OrderStatus = status;
            _context.Orders.Update(order);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cacheKey = $"{_cachePrefix}_{id}";
                string cacheKeyAll = $"{_cachePrefix}_all";
                try
                {
                    await _redis.RemoveAsync(cacheKey);
                    await _redis.RemoveAsync(cacheKeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OrderRepo] Cache operation failed: {ex.Message}");
                }
                return true;
            }
            else
            {
                Console.WriteLine($"[OrderRepo] Failed to update ! ");
                return false;

            }
        }
    }

}
