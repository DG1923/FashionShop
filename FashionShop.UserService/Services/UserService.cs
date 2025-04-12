using FashionShop.UserService.Data;
using FashionShop.UserService.DTOs;
using FashionShop.UserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(UserDbContext context, UserManager<User> userManager,IJwtTokenService jwtTokenService)
        {
            _context = context;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            Guid userGuid = Guid.TryParse(userId, out var temp) ? temp : Guid.Empty;
            if (userGuid == Guid.Empty) {
                throw new ArgumentException("Invalid user ID format. ");

            }

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id==userGuid);
            if (user == null) { 
                throw new ArgumentNullException(nameof(user));
            }
            _context.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            Guid guid = Guid.TryParse(id, out var parsedGuid) ? parsedGuid : Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID format.");
            }
            var user =await _context.Users.FirstOrDefaultAsync(u => u.Id == guid);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<(bool Success, string Message, UserDto User)> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return (false, "User not found", null);
            }
            var token = _jwtTokenService.GenerateToken(user);
            return (true, token, new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        public async Task<(bool Success, string Message, UserDto? User)> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            if(registerUserDto.Email == null || registerUserDto.Password == null || registerUserDto.UserName == null)
            {
                throw new ArgumentNullException("User registration data cannot be null.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerUserDto.Email);
            if (existingUser != null)
            {
                return (false, "User already exists", null);
            }
            var newUser = new User
            {
                UserName = registerUserDto.UserName,
                Email = registerUserDto.Email,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };
            var result = await _userManager.CreateAsync(newUser,registerUserDto.Password);
            if (!result.Succeeded)
            {
                return (false, "User registration failed", null);
            }
            return (true, "User registered successfully", new UserDto
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                Email = newUser.Email
            });

        }

        public async Task<bool> UpdateUserAsync(string userId, UpdateUserDto userDto)
        {
            Guid guid = Guid.TryParse(userId, out var parsedGuid) ? parsedGuid : Guid.Empty;
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID format.");
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == guid);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.UpdatedAt = DateTime.UtcNow;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userDto.Password);
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            else
            {
                throw new Exception("User update failed.");
            }

        }
    }
}
