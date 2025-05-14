using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Models;
using FashionShop.InventoryService.Repository.Interface;

namespace FashionShop.InventoryService.Services.Interface
{
    public interface IInventoryService
    {
        Task<List<Inventory>> GetAll();
        Task<InventoryDisplayDto> GetInventoryByProductIdAsync(Guid productId);
        Task<bool> UpdateInventory(UpdateInventoryDto updateInventoryDto);
        Task<bool> ExternalInventoryExit(Guid inventoryId);
        Task<InventoryDisplayDto> GetByIdAsync(Guid id);

    }
}
