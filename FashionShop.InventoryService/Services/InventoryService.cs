using FashionShop.InventoryService.Data;
using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Models;
using FashionShop.InventoryService.Repository;
using FashionShop.InventoryService.Repository.Interface;
using FashionShop.InventoryService.Services.Interface;

namespace FashionShop.InventoryService.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly IInventoryRepo _repo;
        public InventoryService(IInventoryRepo repo)
        {
            _repo = repo;
            
        }

        public async Task<InventoryDisplayDto> GetInventoryByProductIdAsync(Guid productId)
        {
            try
            {
                var inventory = await _repo.GetByIdAsync(productId);
                if (inventory == null) {
                    return null;
                }
                return new InventoryDisplayDto
                {
                    InventoryId = inventory.Id,
                    ProductId = productId,
                    Quantity = inventory.Quantity,  
                };


            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateInventory(UpdateInventoryDto updateInventoryDto)
        {
            try
            {
                var inventoryExit =await _repo.GetByIdAsync(updateInventoryDto.ProductId);
                if (inventoryExit == null) {
                    inventoryExit = new Inventory
                    {
                        ProductId = updateInventoryDto.ProductId,
                        Quantity = Math.Max(0, updateInventoryDto.QuantityChange),
                        
                    };
                }
                else
                {
                    inventoryExit.Quantity = Math.Max(0,updateInventoryDto.QuantityChange);
                }
                var result =await _repo.UpdateAsync(inventoryExit);
                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex) { 
                Console.WriteLine($"{ex.Message}");
                return false;
                throw;
            }
        }

        public async Task<List<Inventory>> GetAll()
        {
            IEnumerable<Inventory> result = await _repo.GetAllAsync();

            return result.ToList();
        }
    }
}
