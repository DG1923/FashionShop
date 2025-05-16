using FashionShop.CartService.Models;

namespace FashionShop.CartService.Service.Interface
{
    public interface IBaseService<T, TCreate, TUpdate> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllServiceAsync();
        Task<T> GetByIdServiceAsync(Guid id);
        Task<T> CreateServiceAsync(TCreate entity);
        Task<T> UpdateServiceAsync(Guid id, TUpdate entity);
        Task<bool> DeleteServiceAsync(Guid id);
        T MapCreateToEntity(TCreate dto);
        void MapToExistingEntity(T entity, TUpdate dto);
    }
}
