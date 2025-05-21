using FashionShop.OrderService.Model;
using FashionShop.OrderService.Service.Interface;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Service
{
    public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
    {
        private readonly IGenericRepo<T> _genericRepo;

        protected BaseService(IGenericRepo<T> genericRepo)
        {
            _genericRepo = genericRepo ?? throw new ArgumentNullException(nameof(genericRepo));
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                return await _genericRepo.CreateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                return await _genericRepo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var result = await _genericRepo.GetAllAsync();
                return result?.Cast<T>() ?? Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                return await _genericRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                return await _genericRepo.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return false;
            }
        }
    }
}
