namespace FashionShop.CartService.Repo.Interface
{
    public interface IRedisCacheManager
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
