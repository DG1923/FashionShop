
using FashionShop.ProductService.Data;
using FashionShop.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.ProductService.Services
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly DbSet<T> _dbSet;
        private readonly string _cachePrefix;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

        public GenericRepo(ProductDbContext context, IDistributedCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _dbSet = _context.Set<T>(); 
            _cachePrefix = typeof(T).Name.ToLower();
        }
        protected DistributedCacheEntryOptions GetExpirationOptions()
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            return options;
        }
        public async Task<bool> CreateAsync(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await _dbSet.AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                //create cache key
                string cacheKey = $"{_cachePrefix}_{entity.Id}";
                //create expiration time for cache
                var options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = _cacheExpiration
                };
                // Serialize the entity to JSON
                string jsonData = JsonSerializer.Serialize(entity);
                // Store the JSON data in the cache
                await _cache.SetStringAsync(cacheKey, jsonData, options);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var entity =await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<T>().Remove(entity);
            // Save changes to the database
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                return false;
            }
            // Remove the cache entry

            string cacheKey = $"{_cachePrefix}_{id}";
            await _cache.RemoveAsync(cacheKey);
            //remove all cache entries
            string cacheKeyAll = $"{_cachePrefix}_all";
            await _cache.RemoveAsync(cacheKeyAll);
            // Save changes to the database
            
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string cacheKey = $"{_cachePrefix}_all";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize the cached data to the entity type
                IEnumerable<T> cachedEntities = JsonSerializer.Deserialize<IEnumerable<T>>(cachedData);
                if (cachedEntities != null)
                {
                    return cachedEntities;
                }
                throw new ArgumentNullException(cachedData);
            }
            // If not found in cache, fetch from database
            var entities = await _dbSet.ToListAsync();
            //create expiration time for cache
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            // Serialize the entities to JSON
            string jsonData = JsonSerializer.Serialize(entities);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);

            
            return entities;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            string cacheKey = $"{_cachePrefix}_{id}";   
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize the cached data to the entity type
                T cachedEntity = JsonSerializer.Deserialize<T>(cachedData);
                if (cachedEntity != null)
                {
                    return cachedEntity;
                }
                throw new ArgumentNullException(cachedData);
            }
            // If not found in cache, fetch from database

            var entity =await _dbSet.FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }else if(entity != null)
            {
                //create expiration time for cache
                var options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = _cacheExpiration
                };
                // Serialize the entity to JSON
                string jsonData = JsonSerializer.Serialize(entity);
                // Store the JSON data in the cache
                await _cache.SetStringAsync(cacheKey, jsonData, options);

            }
            return entity;
        }


        public async Task<bool> UpdateAsync(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<T>().Update(entity);
            // Save changes to the database
            var result = await _context.SaveChangesAsync();
            if (result < 0)
            {
                return false;
            }
            // Update the cache entry
            string cacheKey = $"{_cachePrefix}_{entity.Id}";
            //create expiration time for cache
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            // Serialize the entity to JSON
            string jsonData = JsonSerializer.Serialize(entity);
            // Store the JSON data in the cache
            await _cache.SetStringAsync(cacheKey, jsonData, options);
            //remove all cache entries
            string cacheKeyAll = $"{_cachePrefix}_all";
            await _cache.RemoveAsync(cacheKeyAll);
            
            return true;        
        
        }

        
    }
}
