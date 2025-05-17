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
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserService(UserDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID format. ");

            }

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                result.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {

            if (id == Guid.Empty)
            {
                Console.WriteLine("Invalid user ID format.");
                return null;
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return null;
            }
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task SeedRoleAndAdminAsync()
        {
            //create roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            //create admin user
            var adminEmail = "admin@gmail.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    NormalizedUserName = "ADMIN",
                    NormalizedEmail = adminEmail.ToUpper(),

                };
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, "Admin@123");
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create admin user.");
                }
            }
        }
        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto userDto)
        {

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID format.");
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
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

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            //remove all roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                throw new Exception("Failed to remove roles.");
            }
            //add new roles

            var addResult = await _userManager.AddToRolesAsync(user, userRole.Roles);
            if (!addResult.Succeeded)
            {
                throw new Exception("Failed to add roles.");
            }
            return addResult.Succeeded;
        }
    }
}
