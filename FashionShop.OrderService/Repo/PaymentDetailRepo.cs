using FashionShop.OrderService.Data;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.OrderService.Repo
{
    public class PaymentDetailRepo : GenericRepo<PaymentDetail>, IPaymentDetailRepo
    {
        private readonly OrderDbContext _context;
        private readonly IRedisCacheManager<PaymentDetail> _redis;
        private readonly DbSet<PaymentDetail> _dbSetPaymentDetail;
        private readonly string _cachePrefix;
        public PaymentDetailRepo(OrderDbContext context, IRedisCacheManager<PaymentDetail> cacheManager) : base(context, cacheManager)
        {
            _context = context;
            _redis = cacheManager;
            _dbSetPaymentDetail = _context.Set<PaymentDetail>();
            _cachePrefix = typeof(PaymentDetail).Name.ToLower();
        }
        public async Task<IEnumerable<PaymentDetail>> GetPaymentDetailsByOrderIdAsync(Guid orderId)
        {
            string cacheKey = $"{_cachePrefix}_order_{orderId}";
            var paymentDetails = await _redis.GetAsync<IEnumerable<PaymentDetail>>(cacheKey);
            if (paymentDetails != null)
            {
                return paymentDetails;
            }
            var paymentDetailsFromDb = await _dbSetPaymentDetail.Where(x => x.OrderId == orderId).ToListAsync();
            if (paymentDetailsFromDb != null)
            {
                try
                {
                    await _redis.SetAsync(cacheKey, paymentDetailsFromDb);
                    Console.WriteLine("[PaymentDetail] Successfully cached payment details for order ID: " + orderId);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Set cache failed");

                }
                return paymentDetailsFromDb;
            }
            else
            {
                Console.WriteLine($"No payment details found for order ID: {orderId}");
                return Enumerable.Empty<PaymentDetail>();
            }
        }
    }


}
