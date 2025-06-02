using FashionShop.OrderService.Model;
using FashionShop.OrderService.Proto;
using FashionShop.OrderService.SyncDataService.GrpcClient.Interface;
using Grpc.Net.Client;

namespace FashionShop.OrderService.SyncDataService.GrpcClient
{
    public class GrpcInventoryClient : IGrpcInventoryClient
    {
        private readonly IConfiguration _configuration;

        public GrpcInventoryClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<(bool success, int quantity)> GetInventoryQuantityAsync(Guid productVariationId)
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcInventory"]);
            if (string.IsNullOrEmpty(_configuration["GrpcInventory"]))
            {
                Console.WriteLine("--> GrpcInventory have not configured !");
                return Task.FromResult((false, 0));
            }
            try
            {
                var client = new GrpcInventoryService.GrpcInventoryServiceClient(channel);
                InventoryRequest inventoryRequest = new InventoryRequest
                {
                    ProductVariationId = productVariationId.ToString()
                };
                var response = client.GetInventoryQuantity(inventoryRequest);
                if (response == null)
                {
                    Console.WriteLine("--> GrpcInventory response is null !");
                    return Task.FromResult((false, 0));
                }
                if (response.Result == true)
                {
                    Console.WriteLine($"--> GrpcInventory true response:{response.Result}_{response.Quantity}_{response.ProductVariationId}");
                    return Task.FromResult((true, response.Quantity));
                }
                else
                {
                    Console.WriteLine($"--> GrpcInventory false response:{response.Result}_{response.Quantity}_{response.ProductVariationId}");
                    return Task.FromResult((false, 0));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call Inventory gRPC Server: {ex.Message}");
                return Task.FromResult((false, 0));
            }



        }

        public Task<bool> UpdateInventoryQuantitiesAsync(List<OrderItem> items)
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcInventory"]);
            if (string.IsNullOrEmpty(_configuration["GrpcInventory"]))
            {
                Console.WriteLine("--> GrpcInventory have not configured !");
                return Task.FromResult(false);
            }
            try
            {
                var client = new GrpcInventoryService.GrpcInventoryServiceClient(channel);
                var inventoryItems = items.Select(item => new OrderItemRequest
                {
                    ProductVariationId = item.ProductVariationId.ToString(),
                    Quantity = item.Quantity
                }).ToList();

                foreach (var item in inventoryItems)
                {
                    Console.WriteLine($"ProductVariationId: {item.ProductVariationId}, Quantity: {item.Quantity}");
                }

                var request = new UpdateInventoryRequest
                {
                    Items = { inventoryItems }
                };
                var response = client.UpdateInventoryQuantity(request);

                return Task.FromResult(response.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call Inventory gRPC Server: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
