using FashionShop.ProductService.Models;
using System.Collections.Generic;

namespace FashionShop.ProductService.Services
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategory(Guid categoryId);  
        Task<Product> GetProductDetail(Guid id);    

    }
}
