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

        // POST: api/CartItem
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItemCreateDto dto)
        {
            if (dto == null)
                return BadRequest("CartItem data is null.");

            // Check product quantity before creating cart item
            var quantityRequest = new GrpcGetQuantityRequest
            {
                ProductId = dto.ProductId.ToString()
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
                ProductId = dto.ProductId.ToString(),
                Quantity = dto.Quantity
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
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItemUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("CartItem data is null.");

            var existingItem = await _cartItemService.GetByIdServiceAsync(id);
            if (existingItem == null)
                return NotFound($"CartItem with ID {id} not found.");

            // Check product quantity before updating
            var quantityRequest = new GrpcGetQuantityRequest
            {
                ProductId = dto.ProductId.ToString()
            };

            var quantityResponse = await _grpcCartItemClient.GetQuantityByProductId(quantityRequest);
            if (quantityResponse == null || !quantityResponse.Result)
                return BadRequest("Failed to verify product quantity.");

            var quantityDifference = dto.Quantity - existingItem.Quantity;
            if (quantityResponse.Quantity < quantityDifference)
                return BadRequest($"Insufficient quantity. Available: {quantityResponse.Quantity}");

            var updated = await _cartItemService.UpdateServiceAsync(id, dto);
            if (updated == null)
                return NotFound($"CartItem with ID {id} not found.");

            // Update product quantity if quantity has changed
            if (quantityDifference != 0)
            {
                var updateRequest = new GrpcUpdateQuantityRequest
                {
                    ProductId = dto.ProductId.ToString(),
                    Quantity = quantityDifference
                };

                var updateResponse = await _grpcCartItemClient.UpdateQuantity(updateRequest);
                if (updateResponse == null || !updateResponse.Result)
                {
                    // Rollback to original quantity
                    await _cartItemService.UpdateServiceAsync(id, new CartItemUpdateDto
                    {
                        ProductId = existingItem.ProductId,
                        Quantity = existingItem.Quantity,
                        ProductName = existingItem.ProductName,
                        Price = existingItem.Price,
                        ImageUrl = existingItem.ImageUrl
                    });
                    return BadRequest("Failed to update product quantity.");
                }
            }

            return Ok(updated);
        }

        // DELETE: api/CartItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _cartItemService.GetByIdServiceAsync(id);
            if (item == null)
                return NotFound($"CartItem with ID {id} not found.");

            // Return quantity to product inventory
            var updateRequest = new GrpcUpdateQuantityRequest
            {
                ProductId = item.ProductId.ToString(),
                Quantity = -item.Quantity // Negative quantity to add back to inventory
            };

            var updateResponse = await _grpcCartItemClient.UpdateQuantity(updateRequest);
            if (updateResponse == null || !updateResponse.Result)
                return BadRequest("Failed to update product quantity.");

            var result = await _cartItemService.DeleteServiceAsync(id);
            if (!result)
                return NotFound($"CartItem with ID {id} not found.");

            return NoContent();
        }
    }
}
