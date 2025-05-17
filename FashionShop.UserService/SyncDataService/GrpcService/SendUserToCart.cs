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
        public override Task<UserResponeIsExitUser> CheckExitUser(CartRequestUserId request, ServerCallContext context)
        {
            Console.WriteLine("SendUserToCart: --> CheckExitUser is checking user ...");
            try
            {
                Guid id = Guid.Parse(request.UserId);
                var user = _userService.GetUserByIdAsync(id).Result;
                var userResponse = new UserResponeIsExitUser()
                {

                    Result = user != null
                };

                return Task.FromResult(userResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> SendUserToCart: --> CheckExitUser error: {ex.Message}");
                return Task.FromResult(new UserResponeIsExitUser()
                {
                    Result = false
                });
            }
        }


    }
}
