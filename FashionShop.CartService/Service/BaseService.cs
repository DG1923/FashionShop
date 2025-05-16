using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service.Interface;

namespace FashionShop.CartService.Service
{
    public class BaseService<T, TCreate, TUpdate> : IBaseService<T, TCreate, TUpdate> where T : BaseEntity
    {
        private readonly IGenericRepo<T> _repo;

        public BaseService(IGenericRepo<T> repo)
        {
            _repo = repo;
        }

        // Create a new entity  
        public async Task<T> CreateServiceAsync(TCreate entityDto)
        {
            if (entityDto == null)
            {
                throw new ArgumentNullException(nameof(entityDto));
            }

            // Map TCreate DTO to T entity  
            T newEntity = MapCreateToEntity(entityDto);

            // Add the entity to the repository  
            await _repo.CreateAsync(newEntity);

            return newEntity;
        }

        // Get all entities  
        public async Task<IEnumerable<T>> GetAllServiceAsync()
        {
            return await _repo.GetAllAsync();
        }

        // Get an entity by ID  
        public async Task<T> GetByIdServiceAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }
            return entity;
        }

        // Update an entity  
        public async Task<T> UpdateServiceAsync(Guid id, TUpdate entityDto)
        {
            if (entityDto == null)
            {
                throw new ArgumentNullException(nameof(entityDto));
            }

            var existingEntity = await _repo.GetByIdAsync(id);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }

            // Map TUpdate DTO to the existing entity  
            MapToExistingEntity(existingEntity, entityDto);

            // Update the entity in the repository  
            await _repo.UpdateAsync(existingEntity);

            return existingEntity;
        }

        // Delete an entity  
        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }

            await _repo.DeleteAsync(entity.Id);
            return true;
        }

        // Helper method to map TCreate DTO to T entity  
        public virtual T MapCreateToEntity(TCreate dto)
        {
            throw new NotImplementedException();
        }

        // Helper method to map TUpdate DTO to an existing T entity  
        public virtual void MapToExistingEntity(T entity, TUpdate dto)
        {
            throw new NotImplementedException();
        }
    }
}

