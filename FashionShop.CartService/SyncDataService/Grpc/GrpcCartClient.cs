using FashionShop.CartService.Protos;
using Grpc.Net.Client;

namespace FashionShop.CartService.SyncDataService.Grpc
{
    public class GrpcCartClient : IGrpcCartClient
    {
        private readonly IConfiguration _configuration;

        public GrpcCartClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<UserResponseToCart> GetExitUserToCreateCart()
        {
            try
            {
                var grpcAddress = _configuration["GrpcSyncUser"];
                if (grpcAddress == null)
                {
                    Console.WriteLine($"Cannot connect Grpc Service");
                    return null;
                }
                Console.WriteLine($"--> Connect Grpc Service : {grpcAddress}");
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new SyncUserToCart.SyncUserToCartClient(channel);
                var request = client.GetExitUser(new Google.Protobuf.WellKnownTypes.Empty());
                var response = request.Users;

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return null;
            }

        }

        public Task<bool> isUserExit(Guid userId)
        {
            try
            {
                var grpcAddress = _configuration["GrpcSyncUser"];
                if (grpcAddress == null)
                {
                    Console.WriteLine($"Cannot connect Grpc Service");
                    return Task.FromResult(false);
                }
                Console.WriteLine($"--> Connect Grpc Service : {grpcAddress}");
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new SyncUserToCart.SyncUserToCartClient(channel);
                var request = client.CheckExitUser(new CartRequestUserId { UserId = userId.ToString() });
                var response = request.Result;
                return Task.FromResult(response); // Fixed: Wrap the boolean response in Task.FromResult
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
