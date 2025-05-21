
using FashionShop.OrderService.Model;

namespace FashionShop.ProductService.Repo.Interface
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
