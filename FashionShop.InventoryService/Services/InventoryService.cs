using FashionShop.InventoryService.AsynDataService;
using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Models;
using FashionShop.InventoryService.Repository.Interface;
using FashionShop.InventoryService.Services.Interface;

namespace FashionShop.InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepo _repo;
        private readonly IMessageBus _messageBus;

        public InventoryService(IInventoryRepo repo, IMessageBus messageBus)
        {
            _repo = repo;
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

        }
        public async Task<InventoryDisplayDto> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id is null");
            }
            var inventory = await _repo.GetByIdAsync(id);
            if (inventory == null)
            {
                return null;
            }

            return new InventoryDisplayDto
            {
                InventoryId = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
            };

        }
        public async Task<InventoryDisplayDto> GetInventoryByProductIdAsync(Guid productId)
        {
            try
            {
                var inventory = await _repo.GetByProductIdAsync(productId);
                if (inventory == null)
                {
                    return null;
                }
                return new InventoryDisplayDto
                {
                    InventoryId = inventory.Id,
                    ProductId = productId,
                    Quantity = inventory.Quantity,
                };


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        //change the way to update inventory
        public async Task<bool> UpdateInventory(UpdateInventoryDto updateInventoryDto)
        {
            try
            {
                var inventory = await _repo.GetByProductIdAsync(updateInventoryDto.ProductId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ProductId = updateInventoryDto.ProductId,
                        Quantity = Math.Max(0, updateInventoryDto.QuantityChange),
                    };
                    return await _repo.CreateAsync(inventory);
                }
                else
                {
                    //change here
                    var newQuantity = inventory.Quantity + updateInventoryDto.QuantityChange;
                    inventory.Quantity = Math.Max(0, newQuantity);
                    var result = await _repo.UpdateAsync(inventory);
                    if (result == true)
                    {
                        //publish event to RabbitMQ
                        try
                        {
                            var publishInventoryDto = new PublishInventoryDto
                            {
                                ProductId = inventory.ProductId,
                                Quantity = inventory.Quantity,
                            };
                            publishInventoryDto.Event = "InventoryPublished";
                            _messageBus.PublishUpdateQuantity(publishInventoryDto);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Cound not send event to Product async ", ex.Message);

                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating inventory", ex.Message);
                return false;
            }
        }

        public async Task<List<Inventory>> GetAll()
        {
            IEnumerable<Inventory> result = await _repo.GetAllAsync();

            return result.ToList();
        }

        public async Task<bool> ExternalInventoryExit(Guid inventoryId)
        {
            var inventory = await _repo.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                return false;
            }
            return true;
        }
    }
}
