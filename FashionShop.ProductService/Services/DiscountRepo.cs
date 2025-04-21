using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Services
{
    public class DiscountRepo: GenericRepo<Discount>, IDiscountRepo
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly DbSet<Discount> _dbSet;
        private readonly string _cachePrefix;

        public DiscountRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _dbSet = _context.Set<Discount>();
            _cachePrefix = typeof(Discount).Name.ToLower();
        }
        public async Task<IEnumerable<Discount>> GetALlDiscountActivate()
        {
            string cacheKey = $"{_cachePrefix}_all_discount_activate";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Discount>>(cachedData);
            }
            var discounts = await _dbSet.Where(d => d.IsActive)
                .ToListAsync();
            if (discounts == null)
            {
                throw new ArgumentNullException(nameof(discounts));
            }
            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the discounts to JSON
            string jsonData = JsonSerializer.Serialize(discounts);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return discounts;   

        }
        public async Task<Discount> GetDiscountByProduct(Guid productId)
        {
            string cacheKey = $"{_cachePrefix}_discount_by_product_{productId}";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<Discount>(cachedData);
            }
            var discount = await _context.Discounts
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.Products.Any(p => p.Id == productId));
            if (discount == null)
            {
                throw new ArgumentNullException(nameof(discount));
            }
            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the discount to JSON
            string jsonData = JsonSerializer.Serialize(discount);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return discount;

        }
    }
}
