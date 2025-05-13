using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Repo.Interface
{
    public interface IProductCategoryRepo : IGenericRepo<ProductCategory>
    {
        Task<ProductCategory> GetCategoryWithSubCategory(Guid id);
        Task<IEnumerable<ProductCategory>> GetAllCategory();



    }
}
