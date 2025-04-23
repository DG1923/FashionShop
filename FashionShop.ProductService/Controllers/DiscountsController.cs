using FashionShop.ProductService.Models;
using FashionShop.ProductService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountRepo _context;

        public DiscountsController(IDiscountRepo context)
        {
            _context = context;
            
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts() {
            var discounts = await _context.GetAllAsync();
            if (discounts == null) { 
                return NotFound();
            }
            return Ok(discounts);
        }
        [HttpGet("/discounts-activate")]
        public async Task<IActionResult> GetAllDiscountsActivate()
        {
            var discounts = await _context.GetALlDiscountActivate();
            if (discounts == null)
            {
                return NotFound();
            }
            return Ok(discounts);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(Guid id)
        {
            var discount = await _context.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }
        [HttpGet("/by-product-id/{productId}")]
        public async Task<IActionResult> GetDiscountByProduct(Guid productId)
        {
            var discount = await _context.GetDiscountByProduct(productId);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDiscount(Discount discount)
        {
            if (discount == null)
            {
                return BadRequest();
            }
            await _context.CreateAsync(discount);
            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.Id }, discount);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(Guid id, Discount discount)
        {
            if (id != discount.Id)
            {
                return BadRequest();
            }
            var existingDiscount = await _context.GetByIdAsync(id);
            if (existingDiscount == null)
            {
                return NotFound();
            }
            // Update the discount properties
            existingDiscount.DiscountPercent = discount.DiscountPercent;
            
            existingDiscount.DeletedAt = discount.DeletedAt;
            existingDiscount.UpdatedAt = DateTime.Now;
            existingDiscount.IsActive = discount.IsActive;
            existingDiscount.Description = discount.Description;


            var result = await _context.UpdateAsync(discount);
            if (result)
            {
                return CreatedAtAction(nameof(GetDiscountById), new { id = discount.Id }, discount);
            }
            return BadRequest("Failed to update discount.");    
        }
        [HttpPut("/activate/{id}")]
        public async Task<IActionResult> ModifyActiveDiscount(Guid id, bool setModifyActivate)
        {
            var discount = await _context.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            discount.IsActive = setModifyActivate;
            var result = await _context.UpdateAsync(discount);
            if (result)
            {
                return CreatedAtAction(nameof(GetDiscountById), new { id = discount.Id }, discount);
            }
            return BadRequest("Failed to update discount.");
        }
            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(Guid id)
        {
            var discount = await _context.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            var result = await _context.DeleteAsync(discount.Id);
            if (!result)
            {
                return BadRequest("Failed to delete discount.");
            }
            
            return NoContent();
        }

    }
}
