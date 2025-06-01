using FashionShop.InventoryService.AsynDataService;
using FashionShop.InventoryService.DTOs;
using FashionShop.InventoryService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FashionShop.InventoryService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;
        private readonly IMessageBus _messageBus;

        public InventoryController(IInventoryService service, IMessageBus messageBus)
        {
            _service = service;

            _messageBus = messageBus;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var inventory = await _service.GetAll();
            return Ok(inventory);
        }
        // GET api/<InventoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDisplayDto>> Get(Guid id)
        {
            try
            {
                var inventory = await _service.GetInventoryByProductIdAsync(id);
                if (inventory == null)
                {
                    return NotFound();
                }
                return new InventoryDisplayDto
                {
                    InventoryId = inventory.InventoryId,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("getInventoryByProductId/{productId}")]
        public async Task<ActionResult<InventoryDisplayDto>> GetByProductId(Guid productId)
        {
            try
            {
                var inventory = await _service.GetInventoryByProductIdAsync(productId);
                if (inventory == null)
                {
                    return NotFound();
                }
                return new InventoryDisplayDto
                {
                    InventoryId = inventory.InventoryId,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        // PUT api/<InventoryController>/5
        [HttpPut("updateInventory")]
        public async Task<ActionResult> Put(UpdateInventoryDto value)
        {
            try
            {

                var result = await _service.UpdateInventory(value);
                if (result == true)
                {

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
