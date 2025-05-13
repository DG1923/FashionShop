using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Repo.Interface
{
    public interface IProductVariationRepo : IGenericRepo<ProductVariation>
    {
        Task<IEnumerable<ProductVariation>> GetProductVariationsByProductId(Guid productId);
        Task<ProductVariation> GetProductVariationDetail(Guid id);
        Task UpdateQuantity(Guid id, int quantity);
    }
}
