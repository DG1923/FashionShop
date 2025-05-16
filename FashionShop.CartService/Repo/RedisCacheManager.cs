using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionShop.CartService.Repo
{
    /// <summary>
    /// Manages Redis cache operations and distributed locking for inventory.
    /// </summary>
    public class RedisCacheManager
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheManager> _logger;
        private readonly TimeSpan _defaultExpirationTime = TimeSpan.FromMinutes(10);

        public RedisCacheManager(IDistributedCache cache, ILogger<RedisCacheManager> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a cached value by key and deserializes it to type T.
        /// </summary>
        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                Console.WriteLine($"[RedisCacheManager] Attempting to get cache for key: {key}");
                var cachedData = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedData))
                {
                    Console.WriteLine($"[RedisCacheManager] Cache miss for key: {key}");
                    return default;
                }
                Console.WriteLine($"--> [RedisCacheManager] get cache success for key: {key}");
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from Redis cache for key: {Key}", key);
                return default;
            }
        }

        /// <summary>
        /// Serializes and stores a value in the cache with an optional expiration.
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpirationTime
                };
                var serializedData = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, serializedData, options);
                Console.WriteLine($"[RedisCacheManager] Set cache for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting data in Redis cache for key: {Key}", key);
            }
        }

        /// <summary>
        /// Removes a value from the cache by key.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                Console.WriteLine($"[RedisCacheManager] Removed cache for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing data from Redis cache for key: {Key}", key);
            }
        }
    }
}
