using FashionShop.ProductService.Models;
using FashionShop.ProductService.Protos;
using Grpc.Net.Client;

namespace FashionShop.ProductService.SyncDataService.GrpcClient
{
    public class ProductProtoClient : IProductProtoClient
    {
        private readonly IConfiguration _configuration;

        public ProductProtoClient(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public InventoryResponse GetQuantity(Guid productId)
        {
            string id = productId.ToString();
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(productId), "Product Id is null or empty");
            }
            Console.WriteLine("--> GrpcProductProto: " + id);
            string grpcAddress = _configuration["GrpcProductProto"];
            var channel = GrpcChannel.ForAddress(grpcAddress);
            var client = new GrpcProduct.GrpcProductClient(channel);
            var request = new ProductRequest { ProductId = id };
            try
            {
                var response = client.GetQuantity(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
            
        }

        public async Task<InventoryUpdateRespone> UpdateQuantity(Guid id, int quantity)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                throw new ArgumentNullException(nameof(id), "Product Id is null or empty");
            }
            if (quantity < 0)
            {
                throw new Exception("Quantity must be greater than 0");
            }
            
            Console.WriteLine("--> GrpcProductProto: " + id);
            string grpcAddress = _configuration["GrpcProductProto"];
            var channel = GrpcChannel.ForAddress(grpcAddress);
            var client = new GrpcProduct.GrpcProductClient(channel);
            var request = new ProductUpdateRequest
            {
                ProductId = id.ToString(),
                Quantity = quantity
            };
            try
            {
                var response =client.UpdateQuantity(request);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new InventoryUpdateRespone { Success = false};
            }
        }
    }
}
