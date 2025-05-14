using FashionShop.InventoryService.Data;
using FashionShop.InventoryService.Models;
using FashionShop.InventoryService.Repository.Interface;
using FashionShop.InventoryService.Services;
using Microsoft.EntityFrameworkCore;
using static Grpc.Core.Metadata;

namespace FashionShop.InventoryService.Repository
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly InventoryDBContext _context;
        private readonly DbSet<Inventory> _dbSet;
        private readonly RedisCacheManager _cacheManager;
        private string _prefix;

        public InventoryRepo(InventoryDBContext context, RedisCacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<Inventory>();
            _cacheManager = cacheManager;
            _prefix = typeof(Inventory).Name.ToLower();

        }
        public async Task<bool> CreateAsync(Inventory entity)
        {
            if (entity == null) {
                throw new ArgumentNullException("Inventory is null");
            }
            await _dbSet.AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0) { 
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (Guid.Empty == id)
                throw new ArgumentNullException("id is null");
            Inventory inventory = _dbSet.FirstOrDefault(x => x.Id == id);
            if (inventory == null)
                return false;
            _dbSet.Remove(inventory);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                // Invalidate cache
                await _cacheManager.RemoveAsync($"{_prefix}_{id}");
                await _cacheManager.RemoveAsync($"{_prefix}_product_{inventory.ProductId}");
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
             string cacheKey = $"{_prefix}_all";    
            var cached = await _cacheManager.GetAsync<IEnumerable<Inventory>>(cacheKey);
            if(cached != null)
                return cached;
            // If not in cache, get from DB
            var inventories = await _dbSet.ToListAsync();
            if (inventories != null)
                await _cacheManager.SetAsync(cacheKey, inventories);
            return await _dbSet.ToListAsync();
        }

        public async Task<Inventory> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException();

            string cacheKey = $"{_prefix}_{id}";
            // Try to get from cache first
            var cached = await _cacheManager.GetAsync<Inventory>(cacheKey);
            if (cached != null)
                return cached;

            // If not in cache, get from DB
            var inventory = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (inventory != null)
                await _cacheManager.SetAsync(cacheKey, inventory);

            return inventory;
        }

        public async Task<Inventory> GetByProductIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentNullException("productId is null");

            string cacheKey = $"{_prefix}_product_{productId}";
            var cached = await _cacheManager.GetAsync<Inventory>(cacheKey);
            if (cached != null)
                return cached;

            var inventory = await _dbSet.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (inventory == null)
                throw new ArgumentException("Inventory not found");

            await _cacheManager.SetAsync(cacheKey, inventory);
            return inventory;
        }


        public async Task<bool> UpdateAsync(Inventory entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                // Invalidate cache
                await _cacheManager.RemoveAsync($"{_prefix}_{entity.Id}");
                await _cacheManager.RemoveAsync($"{_prefix}_product_{entity.ProductId}");
                return true;
            }
            return false;
        }
    }
}
