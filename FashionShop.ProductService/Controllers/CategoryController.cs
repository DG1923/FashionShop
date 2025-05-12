using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services;
using FashionShop.ProductService.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FashionShop.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IProductCategoryRepo _context;  
        public CategoryController(ProductCategoryRepo context)
        {
            _context  = context;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            IEnumerable<ProductCategory> categories = await _context.GetAllCategory();

            IEnumerable<ProductCategoryDisplayDTO> result = categories.Select(category => new ProductCategoryDisplayDTO()
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                
            });
            return Ok(result);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category =await _context.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            var categoryDisplay = new ProductCategoryDisplayDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };
            return Ok(categoryDisplay);
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task<IActionResult> Post(CategoryCreateDTO categoryCreateDTO)
        {
            if (categoryCreateDTO == null)
            {
                return BadRequest("category is null");
            }
            var category = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = categoryCreateDTO.Name,
                ImageUrl = categoryCreateDTO.ImageUrl,
                Description = categoryCreateDTO.Description,    
            };
            bool result = await _context.CreateAsync(category);
            if (result)
            {
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            return BadRequest("Failed to create category.");

        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult?> Put(Guid id, CategoryUpdateDTO value)
        {
            if (Guid.Empty == id)
            {
                return BadRequest("Category is null.");
            }
            var category = await _context.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            category.Name = value.Name;
            category.ImageUrl = value.ImageUrl;
            category.Description = value.Description;

            bool result =await _context.UpdateAsync(category);
            if (result)
            {
                Ok("Category updated successfully.");
            }
            return NotFound("Category not found.");
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if(id == Guid.Empty)
            {
                BadRequest("Invalid category ID.");
            }
            var category = _context.GetByIdAsync(id);
            if (category == null)
            {
                NotFound("Category not found.");
            }
            bool result =await _context.DeleteAsync(id);
            if (result)
            {
                return Ok("Category deleted successfully.");
            }
            else
            {
                return NotFound("Category not found.");
            }
        }
    }
}
