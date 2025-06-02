using FashionShop.OrderService.DTO;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Proto;
using FashionShop.OrderService.Repo.Interface;
using FashionShop.OrderService.Service.Interface;
using FashionShop.OrderService.SyncDataService.GrpcClient.Interface;
using FashionShop.ProductService.Repo.Interface;

namespace FashionShop.OrderService.Service
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderItemRepo _orderItemRepo;
        private readonly IPaymentDetailRepo _paymentDetailRepo;
        private readonly IGrpcCartClient _cartCLient;
        private readonly IGrpcInventoryClient _grpcInventoryClient;
        private readonly IGenericRepo<Order> _orderRepo;

        public OrderService(
            IGenericRepo<Order> orderRepo,
            IOrderItemRepo orderItemRepo,
            IPaymentDetailRepo paymentDetailRepo,
            IGrpcCartClient cartClient,
            IGrpcInventoryClient grpcInventoryClient) : base(orderRepo)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _paymentDetailRepo = paymentDetailRepo;
            _cartCLient = cartClient;
            _grpcInventoryClient = grpcInventoryClient;

        }
        public async Task<bool> CreateOrderAsync(OrderCreateDto order, Guid CartId)
        {
            //validate order and cart items
            if (order == null || order.OrderItems == null || !order.OrderItems.Any() || (CartId == Guid.Empty))
            {
                Console.WriteLine("Invalid order data or empty cart.");
                return false;
            }
            try
            {
                //get cart items from cart service
                var cartItems = await _cartCLient.GetCartItemsAsync(CartId);
                if (cartItems == null || !cartItems.Any())
                {
                    Console.WriteLine("No items found in the cart.");
                    return false;
                }
                //create order 
                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = order.UserId,
                    Status = OrderStatus.Pending,
                    OrderStatus = "Pending",
                    Address = order.Address,


                };

                //create payment detail
                var paymentDetail = new PaymentDetail
                {
                    OrderId = newOrder.Id,
                    PaymentMethod = order.PaymentDetail.PaymentMethod,
                    PaymentStatus = "Pending",
                    TransactionId = order.PaymentDetail.TransactionId ?? string.Empty,
                    PaymentDate = DateTime.UtcNow
                };
                //select items from cart to order
                order.OrderItems = cartItems.Select(item => new OrderItemCreateDto
                {
                    BasePrice = item.BasePrice,
                    ColorCode = item.ColorCode,
                    ColorName = item.ColorName,
                    DiscountId = item.DiscountId != null ? item.DiscountId : Guid.Empty,
                    ImageUrl = item.ImageUrl,
                    ProductColorId = item.ProductColorId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductVariationId = item.ProductVariationId,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    DiscountPercent = item.DiscountPercent ?? 0,
                    CartItemId = item.CartItemId,//this line is bug , I find 2-3 hours to find that :))

                }).ToList();
                foreach (var item in order.OrderItems)
                {
                    //check stock availability before adding to order
                    var result_get = await _grpcInventoryClient.GetInventoryQuantityAsync(item.ProductVariationId);
                    if (result_get.success == false || result_get.quantity < item.Quantity)
                    {
                        Console.WriteLine($"Insufficient stock for product variation {item.ProductVariationId}");
                        return false;
                    }

                }
                List<OrderItem> orderItems = order.OrderItems.Select(item => new OrderItem
                {

                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BasePrice = item.BasePrice,
                    ProductColorId = item.ProductColorId,
                    ColorName = item.ColorName,
                    ColorCode = item.ColorCode,
                    ProductVariationId = item.ProductVariationId,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl,
                    DiscountId = item.DiscountId != Guid.Empty ? item.DiscountId : null,
                    OrderId = newOrder.Id,
                    CartItemId = item.CartItemId//this line is bug , I find 2-3 hours to find that :))
                }).ToList();

                newOrder.OrderItems = orderItems;
                var result_create_order = await _orderRepo.CreateAsync(newOrder);
                if (!result_create_order)
                {
                    Console.WriteLine("Failed to create order.");
                    return false;
                }
                else
                {
                    //delete cart items after order created

                    ClearListCartItemRequest request = new ClearListCartItemRequest();
                    request.CartItemIds.AddRange(newOrder.OrderItems.Select(item => item.CartItemId.ToString()));

                    var result_clear_cart = await _cartCLient.ClearCartAsync(request);
                    if (!result_clear_cart)
                    {
                        //delete order if cart clear failed
                        await _orderRepo.DeleteAsync(newOrder.Id);

                        Console.WriteLine("Failed to clear cart after order creation.");
                        return false;
                    }

                    var result_update = await _grpcInventoryClient.UpdateInventoryQuantitiesAsync(
                        newOrder.OrderItems.Select(item => new OrderItem
                        {
                            ProductId = item.ProductId,
                            ProductVariationId = item.ProductVariationId,
                            Quantity = -item.Quantity, // Negate quantity only for inventory update
                            OrderId = item.OrderId
                        }).ToList()
                    );

                    if (!result_update)
                    {
                        await _orderRepo.DeleteAsync(newOrder.Id);
                        Console.WriteLine("Failed to update inventory quantities.");
                        return false;
                    }
                    newOrder.Total = await CalculateOrderTotalAsync(newOrder.Id);
                    await _orderRepo.UpdateAsync(newOrder);
                    paymentDetail.Amount = newOrder.Total;
                    await _paymentDetailRepo.CreateAsync(paymentDetail);

                    //Reason why I didn't send msg to RabbitMQ is because
                    //I did add feature to update inventory when quantity update in inventory
                    //You can see it in InventoryService in inventoryservice project
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                return false;

            }
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

        //public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        //{
        //    var order = await GetByIdAsync(orderId);
        //    if (order == null) return false;

        //    order.OrderStatus = status;
        //    return await UpdateAsync(order);
        //}

        public async Task<decimal> CalculateOrderTotalAsync(Guid orderId)
        {
            var orderItems = await _orderItemRepo.GetOrderItemsByOrderIdAsync(orderId);
            return orderItems.Sum(item =>
            {
                var discountAmount = item.DiscountPercent.HasValue && item.DiscountPercent > 0
                    ? (item.BasePrice * item.DiscountPercent.Value / 100)
                    : 0;
                return (item.BasePrice - discountAmount) * item.Quantity;
            });
        }
        public async Task<bool> RequestReturnAsync(Guid orderId, ReturnRequestDto request)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Delivered)
                return false;

            order.Status = OrderStatus.ReturnRequested;
            order.ReturnReason = request.Reason;
            order.ReturnRequestDate = DateTime.UtcNow;

            return await UpdateAsync(order);
        }

        public async Task<bool> ProcessReturnRequestAsync(Guid orderId, ReturnReviewDto review)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.ReturnRequested)
                return false;

            order.Status = review.IsApproved ? OrderStatus.ReturnApproved : OrderStatus.ReturnRejected;
            if (!review.IsApproved)
            {
                order.ReturnRejectionReason = review.RejectionReason;
            }

            return await UpdateAsync(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return false;

            // Validate status transitions
            if (!IsValidStatusTransition(order.Status, status))
                return false;

            order.Status = status;
            return await UpdateAsync(order);
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                // Existing transitions
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Confirmed, OrderStatus.Shipping) => true,
                (OrderStatus.Shipping, OrderStatus.Delivered) => true,
                (OrderStatus.Delivered, OrderStatus.ReturnRequested) => true,
                (OrderStatus.ReturnRequested, OrderStatus.ReturnApproved) => true,
                (OrderStatus.ReturnRequested, OrderStatus.ReturnRejected) => true,

                // New transitions for Completed
                (OrderStatus.Delivered, OrderStatus.Completed) => true,      // Complete after successful delivery
                (OrderStatus.ReturnRejected, OrderStatus.Completed) => true, // Complete when return request is rejected

                // New transitions for Cancelled
                (OrderStatus.ReturnApproved, OrderStatus.Cancelled) => true, // Cancel after return request is approved
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,        // Cancel at initial stage
                (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,      // Cancel before shipping

                _ => false
            };
        }

        public async Task<Order> GetOrderStatusWithHistoryAsync(Guid orderId)
        {
            // Get order details including status
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return null;

            // Get associated order items
            order.OrderItems = await _orderItemRepo.GetOrderItemsByOrderIdAsync(orderId);

            // Get payment details
            var paymentDetails = await _paymentDetailRepo.GetPaymentDetailsByOrderIdAsync(orderId);
            order.PaymentDetail = paymentDetails.FirstOrDefault();

            return order;
        }
    }
}
