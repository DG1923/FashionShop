using FashionShop.InventoryService.Services.Interface;
using FashionShop.ProductService.Protos;
using Grpc.Core;

namespace FashionShop.InventoryService.SyncDataService.Grpc.GrpcService
{
    public class GrpcInventoryService : GrpcProduct.GrpcProductBase
    {
        private readonly IInventoryService _service;

        public GrpcInventoryService(IInventoryService service)
        {
            _service = service;
        }
        public override async Task<InventoryResponse> GetQuantity(ProductRequest request, ServerCallContext context)
        {
            Guid id = Guid.Parse(request.ProductId);
            if (id == Guid.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product Id string convert to guid is fail"));
            }
            var inventory = await _service.GetInventoryByProductIdAsync(id);
            if (inventory == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Inventory not found !"));

            }
            return new InventoryResponse
            {
                ProductId = inventory.ProductId.ToString(),
                InventoryId = inventory.InventoryId.ToString(),
                Quantity = inventory.Quantity,
            };
        }
    }
}
