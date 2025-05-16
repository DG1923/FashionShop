using FashionShop.CartService.Data;
using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.CartService.Repo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly CartDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly DbSet<T> _dbSet;
        private readonly string _cachePrefix;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

        public GenericRepo(CartDbContext context, IDistributedCache cache)
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
            if (entity == null)
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
                try
                {
                    Console.WriteLine($"{_cachePrefix}: --> Set cache for creating ...");
                    await _cache.SetStringAsync(cacheKey, jsonData, options);
                    Console.WriteLine($"{_cachePrefix}: --> Set cache success !");
                    string cacheKeyAll = $"{_cachePrefix}_all";
                    Console.WriteLine($"{_cachePrefix}: --> Remove cache all entity... ");
                    await _cache.RemoveAsync(cacheKeyAll);
                    Console.WriteLine($"{_cachePrefix}: --> Remove cache all entity success");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Set cache failed: {ex.Message}");
                }

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _context.Set<T>().Remove(entity);
            // Save changes to the database
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"{_cachePrefix}: --> Delete entity success");
            if (result <= 0)
            {
                return false;
            }
            string cacheKey = $"{_cachePrefix}_{id}";
            string cacheKeyAll = $"{_cachePrefix}_all";
            try
            {

                await _cache.RemoveAsync(cacheKey);
                Console.WriteLine($"{_cachePrefix}:--> Remove cache entity success ");
                await _cache.RemoveAsync(cacheKeyAll);
                Console.WriteLine($"{_cachePrefix}:--> Remove all cache success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Remove cache failed: {ex.Message}");
            }



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
                    Console.WriteLine($"{_cachePrefix}:--> Get cache success");
                    return cachedEntities;
                }
                else
                    throw new ArgumentNullException(cachedData);
            }
            Console.WriteLine($"{_cachePrefix}:--> Get cache failed");
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
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            string cacheKey = $"{_cachePrefix}_{id}";
            string cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize the cached data to the entity type
                T cachedEntity = JsonSerializer.Deserialize<T>(cachedData);
                if (cachedEntity != null)
                {
                    Console.WriteLine($"{_cachePrefix}:--> Get cache data success");
                    return cachedEntity;
                }
                throw new ArgumentNullException(cachedData);
            }
            // If not found in cache, fetch from database
            Console.WriteLine($"{_cachePrefix}:--> Not found data in cache !");
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (entity != null)
            {
                //create expiration time for cache
                var options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = _cacheExpiration
                };
                // Serialize the entity to JSON
                string jsonData = JsonSerializer.Serialize(entity);
                // Store the JSON data in the cache
                try
                {
                    await _cache.SetStringAsync(cacheKey, jsonData, options);
                    Console.WriteLine($"{_cachePrefix}:--> Set Cache data success");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Set cache failed: {ex.Message}");

                }
                return entity;
            }
            return null;
        }


        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
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
            string cacheKeyAll = $"{_cachePrefix}_all";

            //create expiration time for cache
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            // Serialize the entity to JSON
            string jsonData = JsonSerializer.Serialize(entity);
            try
            {

                // Store the JSON data in the cache
                await _cache.SetStringAsync(cacheKey, jsonData, options);
                Console.WriteLine($"{_cachePrefix}:--> Set cache success");
                //remove all cache entries
                await _cache.RemoveAsync(cacheKeyAll);
                Console.WriteLine($"{_cachePrefix}:--> Remove cache all entity success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_cachePrefix}: --> Set cache failed: {ex.Message}");
            }

            return true;

        }


    }
}
