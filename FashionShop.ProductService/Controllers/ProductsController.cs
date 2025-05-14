using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using FashionShop.ProductService.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.ProductService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllServiceAsync();
            if (products == null)
            {
                return NotFound("No products found.");
            }
            List<ProductDisplayDTO> result = new List<ProductDisplayDTO>();
            foreach (var product in products)
            {
                var productDisplay = new ProductDisplayDTO
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
            var product = await _service.GetByIdServiceAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            var productDisplay = new ProductDisplayDTO
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
            
            var product = await _service.GetProductDetailService(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
         
            var products = await _service.GetProductsByCategoryService(categoryId);
            if (products == null || !products.Any())
            {
                return NotFound($"No products found for category ID {categoryId}.");
            }
            return Ok(products);

        }
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateProduct(ProductCreateDTO productCreateDTO)
        {
            var result = await _service.CreateServiceAsync(productCreateDTO);
            if (result !=null)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, productCreateDTO);
            }
            return BadRequest("Failed to create product.");
        }
        [HttpPost("/add-range")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductsRange(List<ProductCreateDetailDTO> list)
        {
        
            IEnumerable<ProductCreateDetailDTO> listResult =await _service.AddRangeProductService(list);
            if (listResult == null || !listResult.Any())
            {
                return BadRequest("Failed to create products.");
            }
            return Ok(listResult);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateNormal productUpdate)
        {
            var result = await _service.UpdateServiceAsync(id,productUpdate);
            if (result!=null)
            {
                return CreatedAtAction(nameof(GetProductById),new { id = id },productUpdate);
            }
            return BadRequest("Failed to update product.");
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]    
        public async Task<IActionResult> DeleteProduct(Guid id)
        { 
            var result = await _service.DeleteServiceAsync(id);
            if (result)
            {
                return NoContent();
            }
            return NotFound($"Product with ID {id} not found.");
        }
    }
}
