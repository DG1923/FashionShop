using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.Protos;
using FashionShop.CartService.Service.Interface;
using Grpc.Core;

namespace FashionShop.CartService.SyncDataService.GrpcServer
{
    public class GrpcCreateCart : CreateNewCartFromNewUser.CreateNewCartFromNewUserBase
    {
        private readonly ICartService _cartService;

        public GrpcCreateCart(ICartService cartService)
        {
            _cartService = cartService;
        }
        public override Task<CartResponseToUser> CreateNewCart(UserRequestToCart request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                Console.WriteLine($"--> [GrpcCreateCart] UserId is null or empty");
                return Task.FromResult(new CartResponseToUser()
                {
                    Result = false
                });
            }
            CartCreateDto cartCreateDto = new CartCreateDto()
            {
                UserId = Guid.Parse(request.UserId),
            };
            var result = _cartService.CreateServiceAsync(cartCreateDto).Result;
            if (result == null)
            {
                Console.WriteLine($"--> [GrpcCreateCart] Create cart failed for user: {request.UserId}");
                return Task.FromResult(new CartResponseToUser()
                {
                    Result = false
                });
            }
            Console.WriteLine($"--> [GrpcCreateCart] Create cart success for user: {request.UserId}");
            return Task.FromResult(new CartResponseToUser()
            {
                Result = true,
            });
        }
    }
}
