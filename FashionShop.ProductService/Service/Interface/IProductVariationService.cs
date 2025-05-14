using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Service.Interface
{
    public interface IProductVariationService
    {
        Task<IEnumerable<ProductVariation>> GetProductVariationsByProductId(Guid productId);
        Task<ProductVariation> GetProductVariationDetail(Guid id);
        Task UpdateQuantity(Guid id, int quantity);
        Task<IEnumerable<ProductVariation>> GetAllAsync();
        Task<ProductVariation> GetByIdAsync(Guid id);
        Task AddAsync(ProductVariation entity);
        Task UpdateAsync(ProductVariation entity);
        Task DeleteAsync(Guid id);
    }
}
