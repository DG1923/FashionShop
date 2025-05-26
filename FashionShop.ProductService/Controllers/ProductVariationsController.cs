using FashionShop.ProductService.Models;
using FashionShop.ProductService.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FashionShop.ProductService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariationsController : ControllerBase
    {
        private readonly IProductVariationService _service;

        public ProductVariationsController(IProductVariationService service)
        {
            _service = service;
        }

        // GET: api/ProductVariations
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var variations = await _service.GetAllAsync();
            if (variations == null || !variations.Any())
                return NotFound("No product variations found.");
            return Ok(variations);
        }

        // GET: api/ProductVariations/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var variation = await _service.GetByIdAsync(id);
            if (variation == null)
                return NotFound($"Product variation with ID {id} not found.");
            return Ok(variation);
        }

        // GET: api/ProductVariations/by-product/{productId}
        [AllowAnonymous]
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            var variations = await _service.GetProductVariationsByProductId(productId);
            if (variations == null || !variations.Any())
                return NotFound($"No variations found for product ID {productId}.");
            return Ok(variations);
        }

        // POST: api/ProductVariations
        [HttpPost]
        public async Task<IActionResult> Create(ProductVariation variation)
        {
            if (variation == null)
                return BadRequest("Invalid product variation data.");
            await _service.AddAsync(variation);
            return CreatedAtAction(nameof(GetById), new { id = variation.Id }, variation);
        }

        // PUT: api/ProductVariations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductVariation variation)
        {
            if (variation == null || id != variation.Id)
                return BadRequest("Invalid product variation data.");
            await _service.UpdateAsync(variation);
            return NoContent();
        }

        // DELETE: api/ProductVariations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // PATCH: api/ProductVariations/{id}/quantity
        [HttpPatch("{id}/quantity")]
        public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] int quantity)
        {
            await _service.UpdateQuantity(id, quantity);
            return NoContent();
        }
    }
}
