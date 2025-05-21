using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.OrderService.Service.Interface;

namespace FashionShop.OrderService.Service
{
    public class PaymentDetailService : BaseService<PaymentDetail>, IPaymentDetailService
    {
        private readonly IPaymentDetailRepo _paymentDetailRepo;

        public PaymentDetailService(IPaymentDetailRepo paymentDetailRepo) : base(paymentDetailRepo)
        {
            _paymentDetailRepo = paymentDetailRepo;
        }

        public async Task<PaymentDetail> GetPaymentDetailByOrderIdAsync(Guid orderId)
        {
            var paymentDetails = await _paymentDetailRepo.GetPaymentDetailsByOrderIdAsync(orderId);
            return paymentDetails.FirstOrDefault();
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status)
        {
            var payment = await GetByIdAsync(paymentId);
            if (payment == null) return false;

            payment.PaymentStatus = status;
            return await UpdateAsync(payment);
        }

        public async Task<bool> ProcessPaymentAsync(PaymentDetail paymentDetail)
        {
            paymentDetail.PaymentDate = DateTime.UtcNow;
            paymentDetail.PaymentStatus = "Processing";
            return await AddAsync(paymentDetail);
        }
    }
}
