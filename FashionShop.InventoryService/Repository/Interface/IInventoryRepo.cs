using FashionShop.InventoryService.Models;

namespace FashionShop.InventoryService.Repository.Interface
{
    public interface IInventoryRepo
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(Inventory entity);
        Task<bool> UpdateAsync(Inventory entity);
        Task<bool> DeleteAsync(Guid id);
        Task<Inventory> GetByProductIdAsync(Guid productId);
    }
}
