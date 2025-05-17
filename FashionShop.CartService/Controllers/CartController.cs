using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.Service.Interface;
using FashionShop.CartService.SyncDataService.Grpc;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IGrpcCartClient _grpcCartClient;

        public CartController(ICartService cartService, IGrpcCartClient grpcCartClient)
        {
            _cartService = cartService;
            _grpcCartClient = grpcCartClient;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllServiceAsync();
            if (carts == null || !carts.Any())
            {
                return NotFound("No carts found.");
            }
            return Ok(carts);
        }

        // GET api/Cart/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(Guid id)
        {
            var cart = await _cartService.GetByIdServiceAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            return Ok(cart);
        }

        // POST api/Cart
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartCreateDto cartCreateDto)
        {
            if (cartCreateDto == null)
            {
                return BadRequest("Cart data is null.");
            }
            var isUserExit = await _grpcCartClient.isUserExit(cartCreateDto.UserId);
            if (!isUserExit)
            {
                return BadRequest("User does not exist.");
            }
            var createdCart = await _cartService.CreateServiceAsync(cartCreateDto);
            if (createdCart == null)
            {
                return BadRequest("Failed to create cart.");
            }

            return CreatedAtAction(nameof(GetCartById), new { id = createdCart.Id }, createdCart);
        }

        // PUT api/Cart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(Guid id, [FromBody] CartUpdateDto cartUpdateDto)
        {
            if (cartUpdateDto == null)
            {
                return BadRequest("Cart data is null.");
            }

            var updatedCart = await _cartService.UpdateServiceAsync(id, cartUpdateDto);
            if (updatedCart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }

            return Ok(updatedCart);
        }

        // DELETE api/Cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(Guid id)
        {
            var result = await _cartService.DeleteServiceAsync(id);
            if (!result)
            {
                return NotFound($"Cart with ID {id} not found.");
            }

            return NoContent();
        }
    }
}
