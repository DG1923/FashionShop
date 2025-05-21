using FashionShop.OrderService.Data;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.OrderService.Model
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly OrderDbContext _context;
        private readonly IRedisCacheManager<T> _cacheManager;
        private readonly DbSet<T> _dbSet;
        private readonly string _cachePrefix;

        public GenericRepo(OrderDbContext context, IRedisCacheManager<T> cacheManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _dbSet = _context.Set<T>();
            _cachePrefix = typeof(T).Name.ToLower();
        }

        public async Task<bool> CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await _dbSet.AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cacheKey = $"{_cachePrefix}_{entity.Id}";
                string cacheKeyAll = $"{_cachePrefix}_all";

                try
                {
                    await _cacheManager.SetAsync(cacheKey, entity);
                    await _cacheManager.RemoveAsync(cacheKeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _dbSet.Remove(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cacheKey = $"{_cachePrefix}_{id}";
                string cacheKeyAll = $"{_cachePrefix}_all";

                try
                {
                    await _cacheManager.RemoveAsync(cacheKey);
                    await _cacheManager.RemoveAsync(cacheKeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string cacheKey = $"{_cachePrefix}_all";
            var cachedEntities = await _cacheManager.GetAsync<IEnumerable<T>>(cacheKey);
            if (cachedEntities != null)
            {
                return cachedEntities;
            }

            var entities = await _dbSet.ToListAsync();
            if (entities != null)
            {
                await _cacheManager.SetAsync(cacheKey, entities);
            }
            return entities;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            string cacheKey = $"{_cachePrefix}_{id}";
            var cachedEntity = await _cacheManager.GetAsync<T>(cacheKey);
            if (cachedEntity != null)
            {
                return cachedEntity;
            }

            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                await _cacheManager.SetAsync(cacheKey, entity);
            }
            return entity;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbSet.Update(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                string cacheKey = $"{_cachePrefix}_{entity.Id}";
                string cacheKeyAll = $"{_cachePrefix}_all";

                try
                {
                    await _cacheManager.SetAsync(cacheKey, entity);
                    await _cacheManager.RemoveAsync(cacheKeyAll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{_cachePrefix}: --> Cache operation failed: {ex.Message}");
                }
                return true;
            }
            return false;
        }
    }
}