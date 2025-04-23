using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Services
{
    public class ProductVariationRepo : GenericRepo<ProductVariation>, IProductVariationRepo
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly DbSet<ProductVariation> _dbSet;
        private readonly string _cachePrefix;
        public ProductVariationRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _dbSet = _context.Set<ProductVariation>();
            _cachePrefix = typeof(ProductVariation).Name.ToLower();

        }
        public Task<ProductVariation> GetProductVariationDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductVariation>> GetProductVariationsByProductId(Guid productId)
        {
        //    if (productId == Guid.Empty)
        //    {
        //        throw new ArgumentNullException(nameof(productId));
        //    }
        //    string cacheKey = $"{_cachePrefix}_by_product_{productId}";
        //    string cachedData = await _cache.GetStringAsync(cacheKey);
        //    if (!string.IsNullOrEmpty(cachedData))
        //    {
        //        return JsonSerializer.Deserialize<IEnumerable<ProductVariation>>(cachedData);
        //    }
        //    var productVariations = _dbSet.Where(p => p.ProductId == productId);
        //    if (productVariations == null)
        //    {
        //        throw new ArgumentNullException(nameof(productVariations));
        //    }
        //    //set the expiration time for cache
        //    var options = GetExpirationOptions();
        //    // Serialize the product variations to JSON
        //    string jsonData = JsonSerializer.Serialize(productVariations);
        //    // Store the JSON data in the cache
        //    await _cache.SetStringAsync(cacheKey, jsonData, options);
        //    return productVariations;
            throw new NotImplementedException();

        }

        public async Task UpdateQuantity(Guid id, int quantity)
        {
            //if (id == Guid.Empty)
            //{
            //    throw new ArgumentNullException(nameof(id));
            //}
            //if (quantity < 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(quantity));
            //}
            //var productVariation = _dbSet.FirstOrDefault(p => p.Id == id);
            //if (productVariation == null)
            //{
            //    throw new ArgumentNullException(nameof(productVariation));
            //}
            //productVariation.Quantity = quantity;
            //_dbSet.Update(productVariation);
            //// Save changes to the database
            //var result = await _context.SaveChangesAsync();
            //if (result < 0)
            //{
            //    throw new Exception("Failed to update product variation quantity");
            //}
            //// Update the cache entry
            //string cacheKey = $"{_cachePrefix}_{id}";
            //// Serialize the updated product variation to JSON
            //string jsonData = JsonSerializer.Serialize(productVariation);
            //// Store the updated JSON data in the cache
            //var options = GetExpirationOptions();
            //await _cache.SetStringAsync(cacheKey, jsonData, options);
            ////remove all cache variations
            //string cacheKeyAll = $"{_cachePrefix}_all_by_product_{productVariation.ProductId}";
            //await _cache.RemoveAsync(cacheKeyAll);
            ////remove all cache entries
            //string cacheKeyAllProduct = $"{_cachePrefix}_all";
            //await _cache.RemoveAsync(cacheKeyAllProduct);
            throw new NotImplementedException();
        }

    }
}

