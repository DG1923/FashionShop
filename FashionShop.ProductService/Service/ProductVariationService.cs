using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using FashionShop.ProductService.Service.Interface;

namespace FashionShop.ProductService.Service
{
    public class ProductVariationService : IProductVariationService
    {
        private readonly IProductVariationRepo _productVariationRepo;

        public ProductVariationService(IProductVariationRepo productVariationRepo)
        {
            _productVariationRepo = productVariationRepo;
        }

        public async Task<IEnumerable<ProductVariation>> GetProductVariationsByProductId(Guid productId)
        {
            return await _productVariationRepo.GetProductVariationsByProductId(productId);
        }

        public async Task<ProductVariation> GetProductVariationDetail(Guid id)
        {
            return await _productVariationRepo.GetProductVariationDetail(id);
        }

        public async Task UpdateQuantity(Guid id, int quantity)
        {
            await _productVariationRepo.UpdateQuantity(id, quantity);

        }

        public async Task<IEnumerable<ProductVariation>> GetAllAsync()
        {
            return await _productVariationRepo.GetAllAsync();
        }

        public async Task<ProductVariation> GetByIdAsync(Guid id)
        {
            return await _productVariationRepo.GetByIdAsync(id);
        }

        public async Task AddAsync(ProductVariation entity)
        {
            await _productVariationRepo.CreateAsync(entity);
        }

        public async Task UpdateAsync(ProductVariation entity)
        {
            await _productVariationRepo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _productVariationRepo.DeleteAsync(id);
        }
    }
}
