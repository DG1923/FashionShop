using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Services.Interface
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);

    }
}
