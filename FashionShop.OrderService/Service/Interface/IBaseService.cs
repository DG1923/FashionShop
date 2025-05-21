namespace FashionShop.OrderService.Service.Interface
{
    public interface IBaseService<T>
    {
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateAsync(T entity);
        Task<bool> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
    }
}
