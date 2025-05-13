using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Services.Interface;
using FashionShop.InventoryService.SyncDataService.Grpc.GrpcClient;

namespace FashionShop.InventoryService.Data
{
    public class PrepDb
    {
        public async static void PrepareQuantity(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScoped = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScoped.ServiceProvider.GetService<ISyncQuantityClient>();
                var quantity = grpcClient.GetQuantity();
                List<UpdateInventoryDto> updateInventoryDtos = new List<UpdateInventoryDto>();
                foreach (var item in quantity)
                {
                    updateInventoryDtos.Add(new UpdateInventoryDto
                    {
                        ProductId = Guid.Parse(item.ProductId),
                        QuantityChange = item.Quantity,
                    });

                }
                await SeedData(serviceScoped.ServiceProvider.GetService<IInventoryService>(), updateInventoryDtos);


            }


        }
        static async Task SeedData(IInventoryService inventoryService, List<UpdateInventoryDto> updateInventoryDtos)
        {
            if (updateInventoryDtos == null || updateInventoryDtos.Count == 0)
            {
                Console.WriteLine("No data to seed");
                return;
            }
            Console.WriteLine("Seeding Data...");
            foreach (var item in updateInventoryDtos)
            {
                //check if inventory exist
                var inventory = await inventoryService.ExternalInventoryExit(item.ProductId);
                if (inventory == false)
                {
                    var result = await inventoryService.UpdateInventory(item);
                    if (result == false)
                    {
                        Console.WriteLine($"Failed to seed inventory for product {item.ProductId}");
                    }
                    else
                    {
                        Console.WriteLine($"Seeded inventory for product {item.ProductId}");
                    }
                }
                else
                {
                    Console.WriteLine($"Inventory for product {item.ProductId} already exist");
                }


            }
        }
    }
}
