using FashionShop.CartService.Service.Interface;
using FashionShop.OrderService.Proto;
using Grpc.Core;

namespace FashionShop.CartService.SyncDataService.GrpcServer
{
    public class GrpcInventoryServer : GrpcCartClientService.GrpcCartClientServiceBase
    {
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;

        public GrpcInventoryServer(
            ICartService cartService,
            ICartItemService cartItemService)
        {
            _cartService = cartService;
            _cartItemService = cartItemService;
        }

        public override async Task<CartItemsResponse> GetCartItems(
            GetCartItemsRequest request,
            ServerCallContext context)
        {
            Console.WriteLine($"--> Getting cart items for cart: {request.CartId}");

            if (string.IsNullOrEmpty(request.CartId))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Cart ID is required"));
            }

            try
            {
                var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(
                    Guid.Parse(request.CartId));

                var response = new CartItemsResponse();
                foreach (var item in cartItems)
                {
                    response.Items.Add(new CartItemModel
                    {
                        ProductId = item.ProductId.ToString(),
                        ProductName = item.ProductName,
                        BasePrice = (double)item.BasePrice,
                        ProductColorId = item.ProductColorId.ToString(),
                        ColorName = item.ColorName,
                        ColorCode = item.ColorCode ?? string.Empty,
                        ProductVariationId = item.ProductVariationId.ToString(),
                        Size = item.Size,
                        Quantity = item.Quantity,
                        ImageUrl = item.ImageUrl,
                        ProductDiscount = item.DiscountId.ToString(),
                        CartItemId = item.Id.ToString()
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error getting cart items: {ex.Message}");
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Error retrieving cart items"));
            }
        }

        public override async Task<ClearCartResponse> ClearCart(
            ClearListCartItemRequest request,
            ServerCallContext context)
        {

            if (!request.CartItemIds.Any())
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Cart ID is required"));
            }
            Console.WriteLine($"--> Start delete cart-item when order created...");

            try
            {

                foreach (var item in request.CartItemIds)
                {
                    if (!Guid.TryParse(item, out Guid cartItemId))
                    {
                        Console.WriteLine($"--> Invalid CartItemId: {item}");
                        continue;
                    }
                    var result = await _cartItemService.RemoveCartItemByIdAsync(cartItemId);
                    if (!result)
                    {
                        Console.WriteLine($"--> Failed to delete cart item: {cartItemId}");
                        return new ClearCartResponse { Success = false };
                    }
                }
                return new ClearCartResponse { Success = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error clearing cart: {ex.Message}");
                return new ClearCartResponse { Success = false };
            }
        }
    }
}
