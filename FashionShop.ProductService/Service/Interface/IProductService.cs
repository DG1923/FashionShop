using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Service.Interface;

public interface IProductService : IBaseService<Product, ProductCreateDTO, ProductUpdateNormal>
{
    Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategoryService(Guid categoryId);
    Task<PagedList<ProductDisplayDTO>> GetProductsByCategoryService(Guid categoryId, int pageNumber, int pageSize = 16);
    Task<ProductDetailsDTO> GetProductDetailService(Guid id);
    Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProductService(List<ProductCreateDetailDTO> list);
    Task<IEnumerable<ProductDisplayDTO>> GetFeaturedProductsService(int take);
    Task<IEnumerable<ProductDisplayDTO>> GetNewProductsService(int take);
    Task<IEnumerable<ProductDisplayDTO>> GetTopDiscountedProductsService(int take);
    Task<PagedList<ProductDisplayDTO>> GetAllProductsPaged(int pageNumber, int pageSize = 16);
    Task<PagedList<ProductDisplayDTO>> SearchProductsService(string searchTerm, int pageNumber = 1, int pageSize = 16);
}
