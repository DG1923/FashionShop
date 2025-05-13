using FashionShop.ProductService.Protos;

namespace FashionShop.ProductService.SyncDataService.GrpcClient
{
    public interface IProductProtoClient
    {
        InventoryResponse GetQuantity(Guid productId);
        Task<InventoryUpdateRespone> UpdateQuantity(Guid id, int quantity);
    }
}
