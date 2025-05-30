using FashionShop.CartService.Protos;
using Grpc.Net.Client;

namespace FashionShop.CartService.SyncDataService.Grpc
{
    public class GrpcCartItemClient : IGrpcCartItemClient
    {
        private readonly IConfiguration _configuration;

        public GrpcCartItemClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<GrpcGetQuantityResponse> GetQuantityByProductId(GrpcGetQuantityRequest grpcGetQuantityRequest)
        {
            if (grpcGetQuantityRequest == null)
            {
                Console.WriteLine($"--> GrpcGetQuantityRequest is null");
                return null;
            }
            try
            {
                var grpcAddress = _configuration["GrpcSyncCartItem"];
                if (grpcAddress == null)
                {
                    Console.WriteLine($"Cannot connect Grpc Service");
                    return null;
                }
                Console.WriteLine($"--> Connect Grpc Service : {grpcAddress}");
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new Protos.GrpcCartItemClient.GrpcCartItemClientClient(channel);
                var response = client.GrpcGetQuantity(grpcGetQuantityRequest);

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return null;
            }
        }

        public Task<GrpcUpdateQuantityResponse> UpdateQuantity(GrpcUpdateQuantityRequest updateQuantityRequest)
        {
            if (_configuration == null)
            {
                Console.WriteLine($"--> GrpcUpdateQuantityRequest is null");
                return null;
            }
            try
            {
                var grpcAddress = _configuration["GrpcSyncCartItem"];
                if (grpcAddress == null)
                {
                    Console.WriteLine($"Cannot connect Grpc Service");
                    return null;
                }
                Console.WriteLine($"--> Connect Grpc Service : {grpcAddress}");
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new Protos.GrpcCartItemClient.GrpcCartItemClientClient(channel);
                var response = client.GrpcUpdateQuantity(updateQuantityRequest);
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return null;
            }
        }
    }
}
