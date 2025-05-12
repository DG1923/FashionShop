using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FashionShop.ProductService.Services
{
    public class ProductCategoryRepo:GenericRepo<ProductCategory>, IProductCategoryRepo
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
        public async Task<IEnumerable<ProductCategory>> GetAllCategory()
        {
            var categories = await _dbSet
                .Include(c => c.SubCategories)
                .ToListAsync();
            return categories;
        }
    
    }
}
