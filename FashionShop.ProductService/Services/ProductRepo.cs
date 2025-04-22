using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Services
{
    public class ProductRepo : GenericRepo<Product>, IProductRepo
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;
       
        public ProductRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _context = context;
            _cache = cache; 
        }

        public async Task<Product> GetProductDetail(Guid id)
        {
            string cacheKey = $"product_details_{id}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if(!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<Product>(cachedData);
            }
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Variations)
                .Include(p => p.Discount)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the product to JSON
            string jsonData = JsonSerializer.Serialize(product);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(Guid categoryId)
        {
            //check if the categoryId is valid
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(categoryId));
            }
            string cacheKey = $"products_by_category_{categoryId}";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Product>>(cachedData);
            }
            var products = await _context.Products
                .Where(p => p.ProductCategoryId == categoryId)
                .ToListAsync();
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the products to JSON
            string jsonData = JsonSerializer.Serialize(products);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return products;



        }
        // Add any additional methods specific to ProductRepo here
    }
}
