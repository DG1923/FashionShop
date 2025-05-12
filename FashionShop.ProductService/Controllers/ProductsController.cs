using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepo _context;

        public ProductsController(IProductRepo context)
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
            List<DTOs.ProductDisplayDTO> result = new List<DTOs.ProductDisplayDTO>();
            foreach (var product in products)
            {
                var productDisplay = new DTOs.ProductDisplayDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    BasePrice = product.BasePrice,
                    MainImageUrl = product.MainImageUrl
                };
                result.Add(productDisplay);
            }
            return Ok(result);
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
            var productDisplay = new DTOs.ProductDisplayDTO
            {
                Id = product.Id,
                Name = product.Name,
                BasePrice = product.BasePrice,
                MainImageUrl = product.MainImageUrl
            };
            return Ok(productDisplay);
        }
        [HttpGet("{id}/details")]   
        public async Task<IActionResult> GetProductDetails(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid product ID.");
            }
            var product = await _context.GetProductDetail(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return BadRequest("Invalid category ID.");
            }
            var products = await _context.GetProductsByCategory(categoryId);
            if (products == null || !products.Any())
            {
                return NotFound($"No products found for category ID {categoryId}.");
            }
            return Ok(products);

        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDTO productCreateDTO)
        {
            if (productCreateDTO == null)
            {
                return BadRequest("Product data is null.");
            }
            Models.Product product = new Models.Product
            {
                Name = productCreateDTO.Name,
                BasePrice = productCreateDTO.Price,
                Description = productCreateDTO.Description,
                MainImageUrl = productCreateDTO.MainImageUrl,
                SKU = productCreateDTO.SKU,
            };
            var result = await _context.CreateAsync(product);
            if (result)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productCreateDTO);
            }
            return BadRequest("Failed to create product.");
        }
        [HttpPost("/add-range")]
        public async Task<IActionResult> CreateProductsRange(List<ProductCreateDetailDTO> list)
        {
            if (list == null || !list.Any())
            {
                return BadRequest("Product data is null.");
            }
            IEnumerable<ProductCreateDetailDTO> listResult =await _context.AddRangeProduct(list);
            if (listResult == null || !listResult.Any())
            {
                return BadRequest("Failed to create products.");
            }
            return Ok(listResult);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateNormal productUpdate)
        {
            if (id == Guid.Empty || productUpdate == null)
            {
                return BadRequest("Invalid product ID or product data.");
            }
           
            var existingProduct = await _context.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            existingProduct.Name = productUpdate.Name;
            existingProduct.BasePrice = productUpdate.Price;
            existingProduct.Description = productUpdate.Description;
            existingProduct.MainImageUrl = productUpdate.MainImageUrl;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            var result = await _context.UpdateAsync(existingProduct);
            if (result)
            {
                return CreatedAtAction(nameof(GetProductById),new { id = existingProduct.Id },productUpdate);
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
