
using FashionShop.UserService.DTOs;

namespace FashionShop.UserService.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto userRole);

        Task<List<UserDto>> GetAllUsersAsync();  
        Task SeedRoleAndAdminAsync();   
    }
}
