using FashionShop.OrderService.DTO;
using FashionShop.OrderService.Model;
using FashionShop.OrderService.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.OrderService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("top-selling")]
        public async Task<ActionResult<IEnumerable<TopSellingProductDto>>> GetTopSellingProducts(
    [FromQuery] int limit = 10,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var topProducts = await _orderService.GetTopSellingProductsAsync(limit);
                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("revenue/total")]
        public async Task<ActionResult<decimal>> GetTotalRevenue([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var revenue = await _orderService.GetTotalRevenueAsync(startDate, endDate);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("revenue/monthly/{year}")]
        public async Task<ActionResult<IDictionary<string, decimal>>> GetMonthlyRevenue(int year)
        {
            try
            {
                var revenue = await _orderService.GetMonthlyRevenueAsync(year);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("revenue/yearly")]
        public async Task<ActionResult<IDictionary<int, decimal>>> GetYearlyRevenue()
        {
            try
            {
                var revenue = await _orderService.GetYearlyRevenueAsync();
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("admin-get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedList<OrderDisplayDto>>> GetAllOrders([FromQuery] string? orderStatus, int pageNumber = 1, [FromQuery] int pageSize = 16)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersPaged(pageNumber, pageSize);

                if (orders == null || !orders.Items.Any())
                {
                    return NotFound("No orders found.");
                }
                if (!string.IsNullOrEmpty(orderStatus))
                {
                    if (Enum.TryParse<OrderStatus>(orderStatus, true, out var status))
                    {
                        orders.Items = orders.Items.Where(o => o.Status == status).ToList();
                    }
                    else
                    {
                        return BadRequest("Invalid order status provided.");
                    }
                }


                return Ok(new
                {
                    CurrentPage = orders.CurrentPage,
                    TotalPages = orders.TotalPages,
                    PageSize = orders.PageSize,
                    TotalCount = orders.TotalCount,
                    HasPrevious = orders.HasPrevious,
                    HasNext = orders.HasNext,
                    Items = orders.Items
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDto>>> GetOrdersByStatus(Guid userId, OrderStatus status)
        {
            try
            {

                var orders = await _orderService.GetOrdersByStatusAsync(status, userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderWithDetailsAsync(id);
                if (order == null)
                    return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(Guid userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateOrder(OrderCreateDto order, Guid cartId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _orderService.CreateOrderAsync(order, cartId);
                if (!result)
                    return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpPut("{id}/status")]
        //public async Task<ActionResult<bool>> UpdateOrderStatus(Guid id, [FromBody] string status)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        //        if (!result)
        //            return BadRequest();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        [HttpGet("{id}/total")]
        public async Task<ActionResult<decimal>> GetOrderTotal(Guid id)
        {
            try
            {

                var total = await _orderService.CalculateOrderTotalAsync(id);
                return Ok(total);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("{id}/return-request")]
        public async Task<ActionResult<bool>> RequestReturn(Guid id, [FromBody] ReturnRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.RequestReturnAsync(id, request);
                if (!result)
                    return BadRequest("Cannot request return for this order");

                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/process-return")]
        [Authorize(Roles = "Admin")] // Only admin can process return requests
        public async Task<ActionResult<bool>> ProcessReturn(Guid id, [FromBody] ReturnReviewDto review)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.ProcessReturnRequestAsync(id, review);
                if (!result)
                    return BadRequest("Cannot process return request");

                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")] // Only admin can update order status
        public async Task<ActionResult<bool>> UpdateOrderStatus(Guid id, [FromBody] OrderStatus status)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, status);
                if (!result)
                    return BadRequest("Cannot update order status");

                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<Order>> GetOrderStatus(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderStatusWithHistoryAsync(id);
                if (order == null)
                    return NotFound();

                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("admin/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                var pendingOrders = orders.Where(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.ReturnRequested);

                return Ok(pendingOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
