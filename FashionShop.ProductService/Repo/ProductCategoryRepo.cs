using FashionShop.ProductService.Data;
using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FashionShop.ProductService.Repo
{
    public class ProductCategoryRepo : GenericRepo<ProductCategory>, IProductCategoryRepo
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly DbSet<ProductCategory> _dbSet;
        private readonly string _cachePrefix;
        public ProductCategoryRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _dbSet = _context.Set<ProductCategory>();
            _cachePrefix = typeof(ProductCategory).Name.ToLower();
        }
        public async Task<ProductCategory> GetCategoryWithSubCategory(Guid id)
        {
            var category = await _dbSet
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            return category;
        }
        public async Task<ProductCategoryDisplayDTO> GetSubCategoryByIdAsync(Guid id)
        {
            string cacheKey = $"{_cachePrefix}_sub_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return System.Text.Json.JsonSerializer.Deserialize<ProductCategoryDisplayDTO>(cachedData);
            }

            var category = await _dbSet
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                Console.WriteLine($"{_cachePrefix}:--> Get subcategory by id failed, category not found");
                return null;
            }
            var subCategoryDisplayDTO = new ProductCategoryDisplayDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                SubCategory = category.SubCategories.Select(subCategory => new ProductCategoryDisplayDTO
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    ImageUrl = subCategory.ImageUrl
                }).ToList()
            };
            var options = GetExpirationOptions();
            try
            {

                var jsonData = System.Text.Json.JsonSerializer.Serialize(subCategoryDisplayDTO);
                await _cache.SetStringAsync(cacheKey, jsonData, options);
                Console.WriteLine($"{_cachePrefix}:--> Get subcategory by id success, cached data");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_cachePrefix}:--> Get subcategory by id failed, {ex.Message}");
            }
            return subCategoryDisplayDTO;


        }
    }
}
