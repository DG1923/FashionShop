using FashionShop.ProductService.Protos;
using FashionShop.ProductService.Repo.Interface;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FashionShop.ProductService.SyncDataService.GrpcService
{
    public class SyncQuantityService : SyncQuantity.SyncQuantityBase
    {
        private readonly IProductVariationRepo _productVariationRepo;

        public SyncQuantityService(IProductVariationRepo productVariationRepo)
        {
            _productVariationRepo = productVariationRepo;
        }
        public async override Task<ListProductResponse> SyncQuantityFromProduct(Empty request, ServerCallContext context)
        {

            var response = new ListProductResponse();
            var productVariation = await _productVariationRepo.GetAllAsync();
            foreach(var item in productVariation)
            {
                ProductResponse product = new ProductResponse
                {
                    ProductId = item.Id.ToString(),
                    Quantity = item.Quantity,
                };
                response.Products.Add(product);

            }
            return response;
        }

    }
}
