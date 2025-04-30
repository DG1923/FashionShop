using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Models;
using System.Collections.Generic;

namespace FashionShop.ProductService.Services
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId);  
        Task<ProductDetailsDTO> GetProductDetail(Guid id);    
        Task<IEnumerable<ProductDetailsDTO>> AddRangeProduct(List<ProductDetailsDTO> list); 

    }
}
