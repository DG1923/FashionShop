
using FashionShop.UserService.DTOs;

namespace FashionShop.UserService.Services
{
    public interface IUserService
    {
        Task<(bool Success,string Message, UserDto User)> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<(bool Success, string Message, UserDto User)> LoginUserAsync(LoginDto loginDto);
        Task<UserDto> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(string userId, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();  
    }
}
