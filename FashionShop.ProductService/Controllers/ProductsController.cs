using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepo<Product> _context;

        public ProductsController(IGenericRepo<Product> context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.GetAllAsync();
            if (products == null)
            {
                return NotFound("No products found.");
            }
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid product ID.");
            }
            var product = await _context.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product data is null.");
            }
            var result = await _context.CreateAsync(product);
            if (result)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            return BadRequest("Failed to create product.");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product product)
        {
            if (id == Guid.Empty || product == null)
            {
                return BadRequest("Invalid product ID or product data.");
            }
            var existingProduct = await _context.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            var result = await _context.UpdateAsync(existingProduct);
            if (result)
            {
                return NoContent();
            }
            return BadRequest("Failed to update product.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid product ID.");
            }
            var result = await _context.DeleteAsync(id);
            if (result)
            {
                return NoContent();
            }
            return NotFound($"Product with ID {id} not found.");
        }
    }
}
