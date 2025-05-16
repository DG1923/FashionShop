using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Protos;
using FashionShop.CartService.Service.Interface;
using FashionShop.CartService.SyncDataService.Grpc;

namespace FashionShop.CartService.Data
{
    public class PrepDb
    {
        public static async Task AddSeedData(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
                var grpcClient = scope.ServiceProvider.GetRequiredService<IGrpcCartClient>();

                try
                {
                    var listUser = grpcClient.GetExitUserToCreateCart()?.ToList();

                    if (listUser == null || !listUser.Any())
                    {
                        Console.WriteLine("--> GRPC: No users retrieved to create carts.");
                        return;
                    }

                    await SeedingData(cartService, listUser);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> GRPC: Error retrieving users from gRPC client: {ex.Message}");
                }
            }
        }

        private static async Task SeedingData(ICartService cartService, List<UserResponseToCart> listUserResponse)
        {
            if (cartService == null)
            {
                Console.WriteLine("--> GRPC: Cart service is null");
                return;
            }
            if (listUserResponse == null)
            {
                Console.WriteLine($"--> GRPC: response for list user is null");
                return;
            }
            Console.WriteLine($"--> GRPC:Start Seeding data for {listUserResponse.Count} users.");
            try
            {
                foreach (var user in listUserResponse)
                {
                    var cart = new CartCreateDto()
                    {
                        UserId = Guid.Parse(user.UserId),
                        Items = new List<CartItemCreateDto>()
                    };
                    var result = await cartService.CreateServiceAsync(cart);
                    if (result != null)
                    {
                        Console.WriteLine($"--> GRPC: Created cart for user {user.UserId} with {cart.Items.Count} items.");
                    }
                    else
                    {
                        Console.WriteLine($"--> GRPC: Failed to create cart for user {user.UserId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> GRPC: Error seeding data: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("--> GRPC: Seeding data completed.");
            }
        }
    }
}
