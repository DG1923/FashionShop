using FashionShop.CartService.Protos;

namespace FashionShop.CartService.SyncDataService.Grpc
{
    public interface IGrpcCartItemClient
    {
        Task<GrpcGetQuantityResponse> GetQuantityByProductId(GrpcGetQuantityRequest grpcGetQuantityRequest);
        Task<GrpcUpdateQuantityResponse> UpdateQuantity(GrpcUpdateQuantityRequest updateQuantityRequest);
    }
}
