using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Repo.Interface
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId);
        Task<ProductDetailsDTO> GetProductDetail(Guid id);
        Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProduct(List<ProductCreateDetailDTO> list);
        // Add new methods
        Task<IEnumerable<ProductDisplayDTO>> GetFeaturedProducts(int take);
        Task<IEnumerable<ProductDisplayDTO>> GetNewProducts(int take);
        Task<IEnumerable<ProductDisplayDTO>> GetTopDiscountedProducts(int take);
    }
}
