using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Repo.Interface
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId);
        Task<PagedList<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId, int pageNumber, int pageSize = 16);
        Task<ProductDetailsDTO> GetProductDetail(Guid id);
        Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProduct(List<ProductCreateDetailDTO> list);
        // Add new methods
        Task<IEnumerable<ProductDisplayDTO>> GetFeaturedProducts(int take);
        Task<IEnumerable<ProductDisplayDTO>> GetNewProducts(int take);
        Task<IEnumerable<ProductDisplayDTO>> GetTopDiscountedProducts(int take);
        Task<PagedList<ProductDisplayDTO>> GetAllProductsPaged(int pageNumber, int pageSize = 16);
        Task<PagedList<ProductDisplayDTO>> SearchProducts(string searchTerm, int pageNumber = 1, int pageSize = 16);
    }
}
