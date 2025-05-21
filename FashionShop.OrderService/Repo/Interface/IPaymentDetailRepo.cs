using FashionShop.OrderService.Model;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Repo.Interface
{
    public interface IPaymentDetailRepo : IGenericRepo<PaymentDetail>
    {
        Task<IEnumerable<PaymentDetail>> GetPaymentDetailsByOrderIdAsync(Guid orderId);
    }

}
