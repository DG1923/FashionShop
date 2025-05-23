using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Service.Interface;

public interface IProductService:IBaseService<Product, ProductCreateDTO, ProductUpdateNormal>
{
    Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategoryService(Guid categoryId);
    Task<ProductDetailsDTO> GetProductDetailService(Guid id);
    Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProductService(List<ProductCreateDetailDTO> list);
}
