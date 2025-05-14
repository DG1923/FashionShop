using FashionShop.ProductService.Data;
using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Repo
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
            Console.WriteLine("--> This func is not implemented yet");
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductVariation>> GetProductVariationsByProductId(Guid productId)
        {
            Console.WriteLine("--> This func is not implemented yet");
            throw new NotImplementedException();
        }

        public async Task UpdateQuantity(Guid id, int quantity)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            var productVariation = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (productVariation == null)
                throw new ArgumentNullException(nameof(productVariation));

            productVariation.Quantity = quantity;
            _dbSet.Update(productVariation);

            var result = await _context.SaveChangesAsync();
            if (result == 0)
                throw new Exception("Failed to update product variation quantity");

            string cacheKey = $"{_cachePrefix}_{id}";
            string jsonData = JsonSerializer.Serialize(productVariation);
            var options = GetExpirationOptions();
            await _cache.SetStringAsync(cacheKey, jsonData, options);

            // Remove related cache keys if navigation properties are loaded
            if (productVariation.ProductColor != null)
            {
                var productColor_id = productVariation.ProductColor.Id;
                string cachePrefixProductColor = typeof(ProductColor).Name.ToLower();
                string cacheKeyProductColor = $"{cachePrefixProductColor}_list_variations_by_{productColor_id}";
                await _cache.RemoveAsync(cacheKeyProductColor);

                if (productVariation.ProductColor.Product != null)
                {
                    var product_id = productVariation.ProductColor.Product.Id;
                    string cachePrefixProduct = typeof(Product).Name.ToLower();
                    string cacheKeyProduct = $"{cachePrefixProduct}_details_{product_id}";
                    await _cache.RemoveAsync(cacheKeyProduct);
                }
            }

            string cacheKeyAllProduct = $"{_cachePrefix}_all";
            await _cache.RemoveAsync(cacheKeyAllProduct);
        }

    }
}

