using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Service.Interface;

public interface IBaseService<T, TCreate, TUpdate> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllServiceAsync();
    Task<T> GetByIdServiceAsync(Guid id);
    Task<T> CreateServiceAsync(TCreate entity);
    Task<T> UpdateServiceAsync(Guid id, TUpdate entity);
    Task<bool> DeleteServiceAsync(Guid id);
    T MapToEntity(TCreate dto);
    void MapToExistingEntity(T entity, TUpdate dto);
}
