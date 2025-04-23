using FashionShop.ProductService.Data;
using FashionShop.ProductService.DTOs;
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
        private readonly DbSet<Models.Product> _dbSet;
        private readonly string _cachePrefix;
        public ProductRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _context = context;
            _cache = cache;
            _dbSet = _context.Set<Models.Product>();
            _cachePrefix = typeof(Models.Product).Name.ToLower();
        }

        public async Task<ProductDetailsDTO> GetProductDetail(Guid id)
        {
            string cacheKey = $"{_cachePrefix}_details_{id}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("--> Get cache product details success !");
                return JsonSerializer.Deserialize<ProductDetailsDTO>(cachedData);
            }
            Console.WriteLine("--> Get cache failed !");
            var product = await _dbSet
                .Include(p => p.ProductCategory)
                .Include(p => p.Discount)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductVariations)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            ProductDetailsDTO productDetails = new ProductDetailsDTO()
            {
                Description = product.Description,
                Id = product.Id,
                MainImageUrl = product.MainImageUrl,
                Name = product.Name,
                BasePrice = product.BasePrice,
                discountDisplayDTO = product.Discount != null ? new DiscountDisplayDTO()
                {
                    Id = product.Discount.Id,
                    DiscountPercent = product.Discount.DiscountPercent,
                    IsActive = product.Discount.IsActive,
                }:null,
                DiscountedPrice = product.Discount != null && product.Discount.IsActive == true?
                    product.BasePrice - (product.BasePrice * product.Discount.DiscountPercent / 100) : null,
                productCategoryDisplayDTO = product.ProductCategory != null ? new ProductCategoryDisplayDTO()
                {
                    Id = product.ProductCategory.Id,
                    Name = product.ProductCategory.Name,
                } : null,
                productColorsDisplayDTO = product.ProductColors != null ? product.ProductColors.Select(pc => new ProductColorDisplayDTO()
                {
                    Id = pc.Id,
                    ColorName = pc.ColorName,
                    ImageUrlColor = pc.ImageUrlColor,
                    productVariationDisplayDTOs = pc.ProductVariations != null ? pc.ProductVariations.Select(pv => new ProductVariationDisplayDTO()
                    {
                        Id = pv.Id,
                        Size = pv.Size,
                        Quantity = pv.Quantity,
                        Description = pv.Description,
                        ImageUrlVariation = pv.ImageUrlVariation
                    }).ToList() : null

                }).ToList() : null

            };
            if(productDetails == null)
            {
                throw new ArgumentNullException(nameof(productDetails));
            }
            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the product to JSON
            string jsonData = JsonSerializer.Serialize(productDetails);
            // Store the JSON data in the cache
            try
            {
                await _cache.SetStringAsync(cacheKey, jsonData, options);
                Console.WriteLine("--> Set cache product details success !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Set cache failed ! {ex.Message}");
                
            }
            return productDetails;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId)
        {
            //check if the categoryId is valid
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(categoryId));
            }
            string cacheKey = $"{_cachePrefix}_by_category_{categoryId}";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<ProductDisplayDTO>>(cachedData);
            }
            var products = await _dbSet
                .Where(p => p.ProductCategoryId == categoryId)
                .Include(p => p.Discount)
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .ToListAsync();

            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            List<ProductDisplayDTO> productDisplayDTO = products.Select(product => new ProductDisplayDTO()
            {
                Id = product.Id,
                Name = product.Name,
                BasePrice = product.BasePrice,
                discountDisplayDTO = product.Discount != null ? new DiscountDisplayDTO()
                {
                    Id = product.Discount.Id,
                    DiscountPercent = product.Discount.DiscountPercent,
                    IsActive = product.Discount.IsActive,
                    Name = product.Discount.Name,
                    
                } : null,
                MainImageUrl = product.MainImageUrl,
                DiscountedPrice = product.Discount != null && product.Discount.IsActive == true?
                    product.BasePrice - (product.BasePrice * product.Discount.DiscountPercent / 100) : null,

            }).ToList();

            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the products to JSON
            string jsonData = JsonSerializer.Serialize(productDisplayDTO);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return productDisplayDTO;



        }
    }
}
