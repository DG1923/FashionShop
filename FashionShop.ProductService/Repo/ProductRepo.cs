using FashionShop.ProductService.Data;
using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.DTOs.DiscountDTO;
using FashionShop.ProductService.DTOs.ProductColorDTO;
using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.DTOs.ProductRatingDTO;
using FashionShop.ProductService.DTOs.ProductVariationDTO;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

namespace FashionShop.ProductService.Repo
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
        public async Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProduct(List<ProductCreateDetailDTO> listProductDetailDTO)
        {
            if (listProductDetailDTO == null || listProductDetailDTO.Count == 0)
            {
                throw new ArgumentNullException(nameof(listProductDetailDTO));
            }

            // Start a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add products
                List<Product> products = new List<Product>();
                foreach (var productDetail in listProductDetailDTO)
                {
                    Product product = new Product()
                    {
                        Id = Guid.NewGuid(),
                        Name = productDetail.Name,
                        Description = productDetail.Description,
                        BasePrice = productDetail.BasePrice,
                        MainImageUrl = productDetail.MainImageUrl,

                    };

                    // Add product subCategory
                    if (productDetail.productCategoryDisplayDTO != null)
                    {
                        ProductCategory category = await _context.ProductCategories
                            .FirstOrDefaultAsync(c => c.Name == productDetail.productCategoryDisplayDTO.Name);

                        if (category != null)
                        {
                            product.ProductCategory = category;
                        }
                        else
                        {
                            ProductCategory newCategory = new ProductCategory()
                            {
                                Name = productDetail.productCategoryDisplayDTO.Name
                            };
                            _context.ProductCategories.Add(newCategory);
                            await _context.SaveChangesAsync();
                            product.ProductCategory = newCategory;
                        }

                    }

                    // Add product colors and variations
                    List<ProductColor> colorList = new List<ProductColor>();
                    foreach (ProductColorCreateDTO productColor in productDetail.productColorsDisplayDTO)
                    {
                        ProductColor color = new ProductColor()
                        {
                            ColorName = productColor.ColorName,
                            ImageUrlColor = productColor.ImageUrlColor,
                            ProductVariations = productColor.productVariationDisplayDTOs.Select(pv => new ProductVariation()
                            {
                                Size = pv.Size,
                                Quantity = pv.Quantity,
                                Description = pv.Description,
                                ImageUrlVariation = pv.ImageUrlVariation
                            }).ToList()
                        };

                        colorList.Add(color);
                    }

                    product.ProductColors = colorList;
                    // Save changes for each product 
                    products.Add(product);
                }

                // Save all changes
                await _dbSet.AddRangeAsync(products);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Clear cache
                string cacheKeyAll = $"{_cachePrefix}_all";
                Console.WriteLine("--> Remove cache all entity...");
                await _cache.RemoveAsync(cacheKeyAll);

                return listProductDetailDTO;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if an error occurs
                await transaction.RollbackAsync();
                Console.WriteLine($"--> Transaction rolled back due to error: {ex.Message}");
                throw;
            }
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
                .Include(p => p.ProductRatings)
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
                } : null,
                DiscountedPrice = product.Discount != null && product.Discount.IsActive == true ?
                    product.BasePrice - (product.BasePrice * product.Discount.DiscountPercent / 100) : null,
                productCategoryDisplayDTO = product.ProductCategory != null ? new ProductCategoryDisplayDTO()
                {
                    Id = product.ProductCategory.Id,
                    Name = product.ProductCategory.Name,
                } : null,
                ProductRatingDisplayDTO = product.ProductRatings != null ? new ProductRatingDisplayDTO
                {
                    ProductRatings = product.ProductRatings
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
            if (productDetails == null)
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
                Console.WriteLine("-->[Product] Get cache products by category success !");
                return JsonSerializer.Deserialize<IEnumerable<ProductDisplayDTO>>(cachedData);
            }
            var products = await _dbSet
                .Where(p => p.ProductCategoryId == categoryId)
                .Include(p => p.Discount)
                .Include(p => p.ProductCategory)
                .Select(MapToProductDisplayDTO())
                .AsNoTracking()
                .ToListAsync();

            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            List<ProductDisplayDTO> productDisplayDTO = products;

            //set the expiration time for cache
            var options = GetExpirationOptions();
            // Serialize the products to JSON
            string jsonData = JsonSerializer.Serialize(productDisplayDTO);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            return productDisplayDTO;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetFeaturedProducts(int take)
        {
            string cacheKey = $"{_cachePrefix}_featured_{take}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("-->[Product] Get cache featured products success !");
                return JsonSerializer.Deserialize<IEnumerable<ProductDisplayDTO>>(cachedData);
            }

            var products = await _dbSet
                .Include(p => p.Discount)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .Select(MapToProductDisplayDTO())
                .ToListAsync();

            if (products != null)
            {
                var options = GetExpirationOptions();
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), options);
            }

            return products;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetNewProducts(int take)
        {
            string cacheKey = $"{_cachePrefix}_new_{take}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("-->[Product] Get cache new products success !");
                return JsonSerializer.Deserialize<IEnumerable<ProductDisplayDTO>>(cachedData);
            }

            var products = await _dbSet
                .Include(p => p.Discount)
                .Include(p => p.ProductRatings)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .Select(MapToProductDisplayDTO())
                .ToListAsync();

            if (products != null)
            {
                var options = GetExpirationOptions();
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), options);
            }

            return products;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetTopDiscountedProducts(int take)
        {
            string cacheKey = $"{_cachePrefix}_top_discounted_{take}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("-->[Product] Get cache top discounted products success !");
                return JsonSerializer.Deserialize<IEnumerable<ProductDisplayDTO>>(cachedData);
            }

            var products = await _dbSet
                .Include(p => p.Discount)
                .Include(p => p.ProductRatings)
                .Where(p => p.Discount != null && p.Discount.IsActive)
                .OrderByDescending(p => p.Discount.DiscountPercent)
                .Take(take)
                .Select(MapToProductDisplayDTO())
                .ToListAsync();

            if (products != null)
            {
                var options = GetExpirationOptions();
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), options);
            }

            return products;
        }

        private static Expression<Func<Product, ProductDisplayDTO>> MapToProductDisplayDTO()
        {
            return p => new ProductDisplayDTO
            {
                Id = p.Id,
                Name = p.Name,
                BasePrice = p.BasePrice,
                MainImageUrl = p.MainImageUrl,
                AverageRating = p.ProductRatings.Any() ? p.ProductRatings.Average(r => r.Rating) : null,
                TotalRating = p.ProductRatings.Any() ? p.ProductRatings.Count() : null,
                discountDisplayDTO = p.Discount != null ? new DiscountDisplayDTO
                {
                    Id = p.Discount.Id,
                    Name = p.Discount.Name,
                    DiscountPercent = p.Discount.DiscountPercent,
                    IsActive = p.Discount.IsActive
                } : null,
                DiscountedPrice = p.Discount != null && p.Discount.IsActive ?
                    p.BasePrice - (p.BasePrice * p.Discount.DiscountPercent / 100) : null
            };
        }

        public async Task<PagedList<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId, int pageNumber, int pageSize = 16)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(categoryId));
            }

            string cacheKey = $"{_cachePrefix}_by_category_{categoryId}_page_{pageNumber}_size_{pageSize}";
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("--> Get cache products by category with pagination success !");
                return JsonSerializer.Deserialize<PagedList<ProductDisplayDTO>>(cachedData);
            }
            var subCategory = await _context.ProductCategories
                .Include(cat => cat.SubCategories)
                .FirstOrDefaultAsync(cat => cat.Id == categoryId);
            if (subCategory == null)
            {
                Console.WriteLine("-->Sub Category not found !");
            }
            var categoryIds = new List<Guid> { categoryId };
            if (subCategory != null)
            {
                categoryIds.AddRange(subCategory.SubCategories.Select(sub => sub.Id).ToList());

            }
            string cacheKeyAll = $"{_cachePrefix}_by_category_{categoryId}_all_filter";
            string cachedDataAll = await _cache.GetStringAsync(cacheKeyAll);
            List<ProductDisplayDTO> cachedDataAllList;

            if (!string.IsNullOrEmpty(cachedDataAll))
            {
                Console.WriteLine("--> Get cache products by category all success!");
                cachedDataAllList = JsonSerializer.Deserialize<List<ProductDisplayDTO>>(cachedDataAll);
            }
            else
            {
                Console.WriteLine("--> Cache miss, fetching from database...");
                cachedDataAllList = await _dbSet
                    .Where(p => categoryIds.Contains(p.ProductCategoryId.Value))
                    .Include(p => p.Discount)
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductRatings)
                    .Select(MapToProductDisplayDTO())
                    .AsNoTracking()
                    .ToListAsync();

                // Only cache if we got data from database
                if (cachedDataAllList != null && cachedDataAllList.Any())
                {
                    await _cache.SetStringAsync(cacheKeyAll,
                        JsonSerializer.Serialize(cachedDataAllList),
                        GetExpirationOptions());
                    Console.WriteLine("--> Set cache products by category all success!");
                }
            }

            var totalCount = cachedDataAllList.Count();
            if (totalCount == 0)
            {
                Console.WriteLine("--> No products found for the specified category !");
                return PagedList<ProductDisplayDTO>.Create(new List<ProductDisplayDTO>(), 0, pageNumber, pageSize);
            }
            var products = cachedDataAllList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var pagedList = PagedList<ProductDisplayDTO>.Create(products, totalCount, pageNumber, pageSize);

            var options = GetExpirationOptions();
            var jsonData = JsonSerializer.Serialize(pagedList);
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            Console.WriteLine("--> Set cache products by category with pagination success !");

            return pagedList;
        }
    }
}
