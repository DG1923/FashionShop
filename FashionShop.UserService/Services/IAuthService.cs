using FashionShop.UserService.DTOs;

namespace FashionShop.UserService.Services
{
    public interface IAuthService
    {
        Task<AuthResponeDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<AuthResponeDto> LoginAsync(LoginDto loginDto);  
    }
}
