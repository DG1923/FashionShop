using FashionShop.ProductService.Data;
using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.DTOs.ProductColorDTO;
using FashionShop.ProductService.DTOs.ProductVariationDTO;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Repo
{
    public class ProductColorRepo : GenericRepo<ProductColor>, IProductColorRepo
    {
        private readonly ProductDbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly DbSet<ProductColor> _dbset;
        private readonly string _cachePrefix;
        public ProductColorRepo(ProductDbContext context, IDistributedCache cache) : base(context, cache)
        {
            _dbContext = context;
            _cache = cache;
            _dbset = _dbContext.Set<ProductColor>();
            _cachePrefix = typeof(ProductColor).Name.ToLower();
        }
        public async Task<List<ProductColorDisplayDTO>> GetVariationsById(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException("id");

            }
            string cacheKey = $"{_cachePrefix}_list_variations_by_{id}";
            string cacheData = await _cache.GetStringAsync(cacheKey); 
            if(!string.IsNullOrEmpty(cacheData))
            {
                return JsonSerializer.Deserialize<List<ProductColorDisplayDTO>>(cacheData);
            }

            var getVariation = _dbContext.Products
                .Include(p=>p.ProductColors)
                    .ThenInclude(p=>p.ProductVariations)
                .FirstOrDefault(p=>p.Id == id);
            if (getVariation != null) { 
                throw new ArgumentNullException("variation by id product is null");
            }
            List<ProductColorDisplayDTO> result = getVariation.ProductColors
                .Select(color => new ProductColorDisplayDTO
                {
                    Id = color.Id,
                    ColorCode = color.ColorCode,
                    ColorName = color.ColorName,
                    ImageUrlColor = color.ImageUrlColor,
                    productVariationDisplayDTOs = color.ProductVariations
                        .Select(variation => new ProductVariationDisplayDTO
                        {
                            Id = variation.Id,
                            Description  = variation.Description,
                            ImageUrlVariation = variation.ImageUrlVariation,
                            Quantity = variation.Quantity,
                            Size = variation.Size,
                        }).ToList(),
                }).ToList();
            var options = GetExpirationOptions();
            string jsonData = JsonSerializer.Serialize(result);
            try
            {
                await _cache.SetStringAsync(cacheKey, jsonData);    
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
                throw;
            }
            return result;
        }
    }
}
