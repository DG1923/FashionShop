using FashionShop.InventoryService.Data;
using FashionShop.InventoryService.Models;
using FashionShop.InventoryService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.InventoryService.Repository
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly InventoryDBContext _context;
        private readonly DbSet<Inventory> _dbSet;

        public InventoryRepo(InventoryDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Inventory>();

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
            if (Guid.Empty == id) {
                throw new ArgumentNullException("id is null");  
            }
            Inventory inventory = _dbSet.FirstOrDefault(x => x.Id == id);
            if (inventory == null) { 
                return false;
            }
            _dbSet.Remove(inventory);
            var result = await _context.SaveChangesAsync();
            if (result > 0) {
                return true;
            }
            return false;   
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
             
            return await _dbSet.ToListAsync();
        }

        public async Task<Inventory> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty) {
                throw new ArgumentNullException();
            }
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(Inventory entity)
        {
            if(entity == null) { throw new ArgumentNullException(nameof(entity)); }
            _dbSet.Update(entity);  
            int result =await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;   
        }
    }
}
