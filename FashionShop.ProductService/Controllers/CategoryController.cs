using FashionShop.ProductService.DTOs;
using FashionShop.ProductService.DTOs.CategoryDTO;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FashionShop.ProductService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IProductCategoryRepo _context;
        public CategoryController(IProductCategoryRepo context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet("GetSubCategories/{id}")]
        public async Task<IActionResult> GetSubCategoriesByIdAsync(Guid id)
        {
            var category = await _context.GetSubCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            var subCategoryDisplayDTO = new ProductCategoryDisplayDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                SubCategory = category.SubCategory == null ? null : category.SubCategory.Select(subCategory => new ProductCategoryDisplayDTO
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    ImageUrl = subCategory.ImageUrl,
                }).ToList()
            };
            return Ok(subCategoryDisplayDTO);
        }
        // GET: api/<CategoryController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            IEnumerable<ProductCategory> categories = await _context.GetAllAsync();

            IEnumerable<ProductCategoryDisplayDTO> result = categories.Select(category => new ProductCategoryDisplayDTO()
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                SubCategory = category.SubCategories == null ? null : category.SubCategories.Select(subCategory => new ProductCategoryDisplayDTO
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    ImageUrl = subCategory.ImageUrl,
                }).ToList()

            });
            return Ok(result);
        }
        [AllowAnonymous]

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _context.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            var categoryDisplay = new ProductCategoryDisplayDTO
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                SubCategory = category.SubCategories.Select(subCategory => new ProductCategoryDisplayDTO
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    ImageUrl = subCategory.ImageUrl,
                }).ToList()
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
        [AllowAnonymous]

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

            // Update basic properties
            category.Name = value.Name;
            category.ImageUrl = value.ImageUrl;
            category.Description = value.Description;

            // Update subcategories if provided
            if (value.SubCategory != null && value.SubCategory.Any())
            {
                // Clear existing subcategories
                category.SubCategories = new List<ProductCategory>();

                // Add new subcategories
                foreach (var subCategoryDto in value.SubCategory)
                {
                    var subCategory = await _context.GetByIdAsync(subCategoryDto.Id);
                    if (subCategory != null)
                    {
                        if (category.SubCategories == null)
                            category.SubCategories = new List<ProductCategory>();

                        if (string.IsNullOrEmpty(subCategory.ImageUrl))
                        {
                            subCategory.ImageUrl = subCategoryDto.ImageUrl;
                        }
                        category.SubCategories.Add(subCategory);
                    }
                }
            }

            bool result = await _context.UpdateAsync(category);
            if (result)
            {
                return Ok("Category updated successfully.");
            }
            return NotFound("Category not found.");
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                BadRequest("Invalid category ID.");
            }
            var category = _context.GetByIdAsync(id);
            if (category == null)
            {
                NotFound("Category not found.");
            }
            bool result = await _context.DeleteAsync(id);
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
