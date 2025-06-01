using FashionShop.OrderService.Proto;
using FashionShop.OrderService.SyncDataService.GrpcClient.Interface;
using Grpc.Net.Client;

namespace FashionShop.OrderService.SyncDataService.GrpcClient
{
    public class GrpcCartClient : IGrpcCartClient
    {
        private readonly IConfiguration _configuration;

        public GrpcCartClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<bool> ClearCartAsync(ClearListCartItemRequest listCartItemId)
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcCart"]);
            if (string.IsNullOrEmpty(_configuration["GrpcCart"]))
            {
                Console.WriteLine("--> GrpcCart have not configured !");
            }
            var client = new GrpcCartClientService.GrpcCartClientServiceClient(channel);

            try
            {


                var response = client.ClearCart(listCartItemId);
                return Task.FromResult(response.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call Cart gRPC Server: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<IEnumerable<Model.OrderItem>> GetCartItemsAsync(Guid cartId)
        {

            Console.WriteLine($"--> Calling Cart gRPC Service {_configuration["GrpcCart"]}");
            if (string.IsNullOrEmpty(_configuration["GrpcCart"]))
            {
                Console.WriteLine("--> GrpcCart have not configured !");
            }
            var channel = GrpcChannel.ForAddress(_configuration["GrpcCart"]);
            //please configure type client
            var client = new GrpcCartClientService.GrpcCartClientServiceClient(channel);
            try
            {
                var request = new GetCartItemsRequest
                {
                    CartId = cartId.ToString()
                };

                var response = client.GetCartItems(request);
                if (response?.Items == null)
                {
                    return new List<Model.OrderItem>();
                }
                foreach (var item in response.Items)
                {
                    Console.WriteLine($"--> Cart Item: {item.ProductName}, Quantity: {item.Quantity}, {item.ProductId},{item.ProductVariationId},{item.ProductColorId}");
                }
                return response.Items.Select(item => new Model.OrderItem
                {
                    ProductId = Guid.TryParse(item.ProductId, out var pid) ? pid : Guid.Empty,
                    ProductName = item.ProductName,
                    BasePrice = (decimal)item.BasePrice,
                    ProductColorId = Guid.TryParse(item.ProductColorId, out var pcid) ? pcid : Guid.Empty,
                    ColorName = item.ColorName,
                    ColorCode = item.ColorCode,
                    ProductVariationId = Guid.TryParse(item.ProductVariationId, out var pvid) ? pvid : Guid.Empty,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl,
                    DiscountId = Guid.TryParse(item.ProductDiscount, out var did) ? did : (Guid?)null,
                    CartItemId = Guid.TryParse(item.CartItemId, out var cartItemId) ? cartItemId : Guid.Empty
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call Cart gRPC Server: {ex.Message}");
                return new List<Model.OrderItem>();
            }
        }


    }
}
