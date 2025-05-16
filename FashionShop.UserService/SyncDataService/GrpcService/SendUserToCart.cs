using FashionShop.CartService.Protos;
using FashionShop.UserService.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FashionShop.UserService.SyncDataService.GrpcService
{
    public class SendUserToCart : SyncUserToCart.SyncUserToCartBase
    {
        private readonly IUserService _userService;

        public SendUserToCart(IUserService userService)
        {
            _userService = userService;
        }
        public override Task<ListUserResponseToCart> GetExitUser(Empty request, ServerCallContext context)
        {
            Console.WriteLine("SendUserToCart: --> GetExitUser is getting users ...");
            var users = _userService.GetAllUsersAsync().Result;
            var filter_user =
            var listUserResponse = new ListUserResponseToCart();
            foreach (var user in users)
            {
                listUserResponse.Users.Add(new UserResponseToCart()
                {
                    UserId = user.Id.ToString(),
                });
            }
            Console.WriteLine($"SendUserToCart: --> GetExitUser was called with {users.Count} users");
            return Task.FromResult(listUserResponse);
        }

    }
}
