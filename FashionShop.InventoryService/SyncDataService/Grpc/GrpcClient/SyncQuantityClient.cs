using FashionShop.ProductService.Protos;
using Grpc.Net.Client;

namespace FashionShop.InventoryService.SyncDataService.Grpc.GrpcClient
{
    public class SyncQuantityClient : ISyncQuantityClient
    {
        private readonly IConfiguration _configuration;

        public SyncQuantityClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        IEnumerable<ProductResponse> ISyncQuantityClient.GetQuantity()
        {
            var grpcAddress = _configuration["GrpcSyncQuantity"];
            if (grpcAddress == null)
            {
                throw new ArgumentNullException("GrpcSyncQuantity is null " + nameof(grpcAddress));
            }
            Console.WriteLine("--> GrpcSyncQuantity: " + grpcAddress);
            var channel = GrpcChannel.ForAddress(grpcAddress);
            var client = new SyncQuantity.SyncQuantityClient(channel);
            var request = client.SyncQuantityFromProduct(new Google.Protobuf.WellKnownTypes.Empty());
            var response = request.Products;
            return response;
        }
    }
}
