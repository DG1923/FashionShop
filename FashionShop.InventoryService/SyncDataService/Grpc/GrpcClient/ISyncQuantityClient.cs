using FashionShop.ProductService.Protos;

namespace FashionShop.InventoryService.SyncDataService.Grpc.GrpcClient
{
    public interface ISyncQuantityClient
    {
        IEnumerable<ProductResponse> GetQuantity();
    }
}
