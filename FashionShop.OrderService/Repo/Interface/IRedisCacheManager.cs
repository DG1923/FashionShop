namespace FashionShop.OrderService.Repo.Interface
{
    public interface IRedisCacheManager<T>
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
