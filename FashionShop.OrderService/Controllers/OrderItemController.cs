using FashionShop.OrderService.Model;
using FashionShop.OrderService.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(Guid orderId)
        {
            var items = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> AddOrderItem(OrderItem orderItem)
        {
            var result = await _orderItemService.AddAsync(orderItem);
            if (!result)
                return BadRequest();
            return Ok(result);
        }

        [HttpPut("{id}/quantity")]
        public async Task<ActionResult<bool>> UpdateQuantity(Guid id, [FromBody] int quantity)
        {
            var result = await _orderItemService.UpdateQuantityAsync(id, quantity);
            if (!result)
                return BadRequest();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteOrderItem(Guid id)
        {
            var result = await _orderItemService.DeleteOrderItemAsync(id);
            if (!result)
                return NotFound();
            return Ok(result);
        }
    }
}
