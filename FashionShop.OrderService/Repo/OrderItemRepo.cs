using FashionShop.OrderService.Data;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.OrderService.Repo
{
    public class OrderItemRepo : GenericRepo<OrderItem>, IOrderItemRepo
    {
        private readonly OrderDbContext _context;
        private readonly IRedisCacheManager<OrderItem> _redis;
        private readonly DbSet<OrderItem> _dbSetOrderItem;
        private readonly string _cachePrefix;
        public OrderItemRepo(OrderDbContext context, IRedisCacheManager<OrderItem> cacheManager) : base(context, cacheManager)
        {
            _context = context;
            _redis = cacheManager;
            _dbSetOrderItem = _context.Set<OrderItem>();
            _cachePrefix = typeof(OrderItem).Name.ToLower();


        }

        public async Task<bool> DeleteOrderItemAsync(Guid orderItemId)
        {
            var orderItem = _dbSetOrderItem.Find(orderItemId);
            if (orderItem == null)
            {
                return false;
            }
            _dbSetOrderItem.Remove(orderItem);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cachekeyAll = $"{_cachePrefix}_all";
                string cachekey = $"{_cachePrefix}_{orderItemId}";
                try
                {
                    await _redis.RemoveAsync(cachekey);
                    await _redis.RemoveAsync(cachekeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            string cacheKey = $"{_cachePrefix}_order_{orderId}";
            var orderItems = await _redis.GetAsync<IEnumerable<OrderItem>>(cacheKey);
            if (orderItems != null)
            {
                return orderItems;
            }
            var orderItemsFromDb = await _dbSetOrderItem.Where(x => x.OrderId == orderId).ToListAsync();
            if (orderItemsFromDb != null)
            {
                try
                {
                    await _redis.SetAsync(cacheKey, orderItemsFromDb);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }
            }
            return orderItemsFromDb;
        }
        public async Task<bool> UpdateOrderItemQuantityAsync(Guid orderItemId, int quantity)
        {
            var orderItem = await _dbSetOrderItem.FindAsync(orderItemId);
            if (orderItem == null)
            {
                return false;
            }
            orderItem.Quantity = quantity;
            _dbSetOrderItem.Update(orderItem);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cachekeyAll = $"{_cachePrefix}_all";
                string cachekey = $"{_cachePrefix}_{orderItemId}";
                try
                {
                    await _redis.RemoveAsync(cachekey);
                    await _redis.RemoveAsync(cachekeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }
                return true;
            }
            return false;
        }
    }

}
