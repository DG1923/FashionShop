using FashionShop.OrderService.Model;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.OrderService.Service.Interface;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Service
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderItemRepo _orderItemRepo;
        private readonly IPaymentDetailRepo _paymentDetailRepo;
        private readonly IGenericRepo<Order> _orderRepo;

        public OrderService(
            IGenericRepo<Order> orderRepo,
            IOrderItemRepo orderItemRepo,
            IPaymentDetailRepo paymentDetailRepo) : base(orderRepo)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _paymentDetailRepo = paymentDetailRepo;
        }

        public async Task<Order> GetOrderWithDetailsAsync(Guid orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                order.OrderItems = await _orderItemRepo.GetOrderItemsByOrderIdAsync(orderId);
                var paymentDetails = await _paymentDetailRepo.GetPaymentDetailsByOrderIdAsync(orderId);
                order.PaymentDetail = paymentDetails.FirstOrDefault();
            }
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await GetAllAsync();
            return orders.Where(o => o.UserId == userId);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = status;
            return await UpdateAsync(order);
        }

        public async Task<decimal> CalculateOrderTotalAsync(Guid orderId)
        {
            var orderItems = await _orderItemRepo.GetOrderItemsByOrderIdAsync(orderId);
            return orderItems.Sum(item => item.Price * item.Quantity);
        }
    }
}
