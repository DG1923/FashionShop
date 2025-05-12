using FashionShop.ProductService.Models;

namespace FashionShop.ProductService.Services.Interface
{
    public interface IDiscountRepo : IGenericRepo<Discount>
    {
        Task<IEnumerable<Discount>> GetALlDiscountActivate();
        Task<Discount> GetDiscountByProduct(Guid productId);
    }
}
