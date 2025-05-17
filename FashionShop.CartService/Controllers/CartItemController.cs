using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
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

        // POST: api/CartItem
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItemCreateDto dto)
        {
            if (dto == null)
                return BadRequest("CartItem data is null.");

            var created = await _cartItemService.CreateServiceAsync(dto);
            if (created == null)
                return BadRequest("Failed to create CartItem.");

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/CartItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItemUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("CartItem data is null.");

            var updated = await _cartItemService.UpdateServiceAsync(id, dto);
            if (updated == null)
                return NotFound($"CartItem with ID {id} not found.");

            return Ok(updated);
        }

        // DELETE: api/CartItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _cartItemService.DeleteServiceAsync(id);
            if (!result)
                return NotFound($"CartItem with ID {id} not found.");

            return NoContent();
        }
    }
}
