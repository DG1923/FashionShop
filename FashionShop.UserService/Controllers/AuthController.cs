using FashionShop.UserService.DTOs;
using FashionShop.UserService.Services;
using FashionShop.UserService.SyncDataService.GrpcClient;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISendNewUser _sendNewUser;

        public AuthController(IAuthService authService, ISendNewUser sendNewUser)
        {
            _authService = authService;
            _sendNewUser = sendNewUser;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(loginDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(registerUserDto);
            if (result.Success)
            {
                await _sendNewUser.SendNewUser(result.UserId);
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
