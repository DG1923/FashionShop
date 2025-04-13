using FashionShop.UserService.DTOs;
using FashionShop.UserService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto userDto)
        {
           if(userId == Guid.Empty || userDto == null)
            {
                return BadRequest("Invalid user ID or user data.");
            }
           var result = await _userService.UpdateUserAsync(userId, userDto);
            if (result)
            {
                return NoContent();
            }
            return NotFound("User not found.");
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            var result = await _userService.DeleteUserAsync(userId);
            if (result)
            {
                return NoContent();
            }
            return NotFound("User not found.");
        }
        [HttpPost("update-role")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRoleDto userRole)
        {
            if (userRole == null)
            {
                return BadRequest("Invalid user role data.");
            }
            var result = await _userService.UpdateUserRoleAsync(userRole);
            if (result)
            {
                return NoContent();
            }
            return NotFound("User not found.");
        }
        [HttpPost("seed")]
        public async Task<IActionResult> SeedRoleAndAdmin()
        {
            await _userService.SeedRoleAndAdminAsync();
            return Ok("Roles and admin user seeded successfully.");
        }
    }
}
