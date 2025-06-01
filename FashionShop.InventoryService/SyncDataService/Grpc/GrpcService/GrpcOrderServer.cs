using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Services.Interface;
using FashionShop.OrderService.Proto;
using Grpc.Core;

namespace FashionShop.InventoryService.SyncDataService.Grpc.GrpcService
{
    public class GrpcOrderServer : GrpcInventoryService.GrpcInventoryServiceBase
    {
        private readonly IInventoryService _inventoryService;

        public GrpcOrderServer(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public override async Task<InventoryResponse> GetInventoryQuantity(
            InventoryRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.ProductVariationId, out Guid productId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "Invalid product ID format"));
            }

            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);

            return new InventoryResponse
            {
                ProductVariationId = request.ProductVariationId,
                Quantity = inventory?.Quantity ?? 0,
                Result = inventory != null
            };
        }

        public override async Task<UpdateInventoryResponse> UpdateInventoryQuantity(
            UpdateInventoryRequest request, ServerCallContext context)
        {
            try
            {
                foreach (var item in request.Items)
                {
                    if (!Guid.TryParse(item.ProductVariationId, out Guid productId))
                    {
                        return new UpdateInventoryResponse
                        {
                            Success = false,
                            Message = $"Invalid product ID format: {item.ProductVariationId}"
                        };
                    }
                    UpdateInventoryDto updateInventoryDto = new UpdateInventoryDto
                    {
                        ProductId = productId,
                        QuantityChange = item.Quantity
                    };

                    var success = await _inventoryService.UpdateInventory(updateInventoryDto);

                    if (!success)
                    {
                        return new UpdateInventoryResponse
                        {
                            Success = false,
                            Message = $"Failed to update quantity for product {item.ProductVariationId}"
                        };
                    }
                }

                return new UpdateInventoryResponse
                {
                    Success = true,
                    Message = "All inventory updates successful"
                };
            }
            catch (Exception ex)
            {
                return new UpdateInventoryResponse
                {
                    Success = false,
                    Message = $"Error updating inventory: {ex.Message}"
                };
            }
        }
    }
}
