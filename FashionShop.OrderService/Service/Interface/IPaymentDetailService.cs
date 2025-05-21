using FashionShop.OrderService.Model;

namespace FashionShop.OrderService.Service.Interface
{
    public interface IPaymentDetailService : IBaseService<PaymentDetail>
    {
        Task<PaymentDetail> GetPaymentDetailByOrderIdAsync(Guid orderId);
        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status);
        Task<bool> ProcessPaymentAsync(PaymentDetail paymentDetail);
    }
}
