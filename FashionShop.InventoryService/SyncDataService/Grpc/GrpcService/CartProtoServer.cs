using FashionShop.CartService.Protos;
using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Services.Interface;
using Grpc.Core;

namespace FashionShop.InventoryService.SyncDataService.Grpc.GrpcService
{
    public class CartProtoServer : GrpcCartItemClient.GrpcCartItemClientBase
    {
        private readonly IInventoryService _service;
        public CartProtoServer(IInventoryService service)
        {
            _service = service;
        }
        public override Task<GrpcGetQuantityResponse> GrpcGetQuantity(GrpcGetQuantityRequest request, ServerCallContext context)
        {
            if (request == null)
            {
                Console.WriteLine($"--> GrpcGetQuantityRequest is null");
                return Task.FromResult(new GrpcGetQuantityResponse
                {
                    Result = false,
                });
            }
            try
            {
                Guid id = Guid.Empty;
                bool result = Guid.TryParse(request.ProductId, out id);
                if (result == false || string.IsNullOrEmpty(request.ProductId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Product Id string convert to guid is fail"));
                }
                var inventory = _service.GetInventoryByProductIdAsync(id).Result;
                if (inventory == null)
                {
                    Console.WriteLine($"--> Inventory not found !");
                    return Task.FromResult(new GrpcGetQuantityResponse
                    {
                        Result = false,
                    });
                }
                return Task.FromResult(new GrpcGetQuantityResponse
                {
                    ProductId = inventory.ProductId.ToString(),
                    Quantity = inventory.Quantity,
                    Result = true,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return Task.FromResult(new GrpcGetQuantityResponse
                {
                    Result = false,
                });
            }
        }
        public override Task<GrpcUpdateQuantityResponse> GrpcUpdateQuantity(GrpcUpdateQuantityRequest request, ServerCallContext context)
        {
            if (request == null)
            {
                Console.WriteLine("--> GrpcUpdateQuantityRequest is null");
                return Task.FromResult(new GrpcUpdateQuantityResponse
                {
                    Result = false,
                });
            }
            try
            {
                Guid id = Guid.Empty;
                bool result = Guid.TryParse(request.ProductId, out id);
                if (result == false || string.IsNullOrEmpty(request.ProductId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Product Id string convert to guid is fail"));
                }
                var inventory = _service.GetInventoryByProductIdAsync(id).Result;
                if (inventory == null)
                {
                    Console.WriteLine($"--> Inventory not found !");
                    return Task.FromResult(new GrpcUpdateQuantityResponse
                    {
                        Result = false,
                    });
                }
                if (request.Quantity >= 0 && request.Quantity <= inventory.Quantity)
                {
                    inventory.Quantity -= request.Quantity;
                }
                else if (request.Quantity < 0)
                {
                    inventory.Quantity += Math.Abs(request.Quantity);
                }
                else
                {
                    Console.WriteLine($"--> Quantity is not valid !");
                    return Task.FromResult(new GrpcUpdateQuantityResponse
                    {
                        Result = false,
                    });
                }
                UpdateInventoryDto updateInventoryDto = new UpdateInventoryDto
                {
                    ProductId = Guid.Parse(request.ProductId),
                    QuantityChange = inventory.Quantity,
                };
                var resultUpdate = _service.UpdateInventory(updateInventoryDto).Result;
                return Task.FromResult(new GrpcUpdateQuantityResponse
                {
                    Result = resultUpdate,
                    ProductId = inventory.ProductId.ToString(),
                    Quantity = inventory.Quantity,
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error when connect Grpc Service: {ex.Message}");
                return Task.FromResult(new GrpcUpdateQuantityResponse
                {
                    Result = false,
                });
            }
        }
    }

}
