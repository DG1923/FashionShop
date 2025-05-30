using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Protos;
using FashionShop.CartService.Service.Interface;
using FashionShop.CartService.SyncDataService.Grpc;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly IGrpcCartItemClient _grpcCartItemClient;

        public CartItemController(
            ICartItemService cartItemService,
            IGrpcCartItemClient grpcCartItemClient)
        {
            _cartItemService = cartItemService;
            _grpcCartItemClient = grpcCartItemClient;
        }

        // GET: api/CartItem
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _cartItemService.GetAllServiceAsync();
            return Ok(items);
        }

        // GET: api/CartItem/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _cartItemService.GetByIdServiceAsync(id);
            if (item == null)
                return NotFound($"CartItem with ID {id} not found.");
            return Ok(item);
        }

        // GET: api/CartItem/quantity/{productId}
        [HttpGet("quantity/{productId}")]
        public async Task<IActionResult> GetQuantityByProductId(Guid productId)
        {
            var request = new GrpcGetQuantityRequest
            {
                ProductId = productId.ToString()
            };

            var response = await _grpcCartItemClient.GetQuantityByProductId(request);
            if (response == null)
                return BadRequest("Failed to get quantity from product service.");

            return Ok(new
            {
                ProductId = productId,
                Quantity = response.Quantity,
                Success = response.Result,
            });
        }

        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetCartItems(Guid cartId)
        {
            var items = await _cartItemService.GetCartItemsByCartIdAsync(cartId);
            if (!items.Any())
                return NotFound($"No items found in cart {cartId}");
            return Ok(items);
        }

        // POST: api/CartItem
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItemCreateDto dto)
        {
            if (dto == null || dto.Quantity <= 0)
                return BadRequest("CartItem data is null.");

            // Check product quantity before creating cart item
            var quantityRequest = new GrpcGetQuantityRequest
            {
                ProductId = dto.ProductVariationId.ToString()
            };

            var quantityResponse = await _grpcCartItemClient.GetQuantityByProductId(quantityRequest);
            //check if quantityResponse is null or result is false
            if (quantityResponse == null || !quantityResponse.Result)
                return BadRequest("Failed to verify product quantity.");

            if (quantityResponse.Quantity < dto.Quantity)
                return BadRequest($"Insufficient quantity. Available: {quantityResponse.Quantity}");

            var created = await _cartItemService.CreateServiceAsync(dto);
            if (created == null)
                return BadRequest("Failed to create CartItem.");

            // Update product quantity
            var updateRequest = new GrpcUpdateQuantityRequest
            {
                ProductId = dto.ProductVariationId.ToString(),
                Quantity = quantityResponse.Quantity - dto.Quantity
            };

            var updateResponse = await _grpcCartItemClient.UpdateQuantity(updateRequest);
            if (updateResponse == null || !updateResponse.Result)
            {
                // Rollback cart item creation if quantity update fails
                await _cartItemService.DeleteServiceAsync(created.Id);
                return BadRequest("Failed to update product quantity.");
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/CartItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItemUpdateDto updateDto)
        {
            var result = await _cartItemService.UpdateCartItemByIdAsync(id, updateDto);
            if (result == null)
                return BadRequest("Failed to update cart item");
            return Ok(result);
        }

        // DELETE: api/CartItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _cartItemService.RemoveCartItemByIdAsync(id);
            if (!result)
                return NotFound($"CartItem {id} not found or could not be removed");
            return NoContent();
        }
    }
}
