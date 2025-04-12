using FashionShop.UserService.Data;
using FashionShop.UserService.DTOs;
using FashionShop.UserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FashionShop.UserService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager; // Change UserDbContext to User
        private readonly RoleManager<IdentityRole<Guid>> _roleManager; // Change UserDbContext to IdentityRole<Guid>
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager, // Change UserDbContext to User
            RoleManager<IdentityRole<Guid>> roleManager, // Change UserDbContext to IdentityRole<Guid>
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<AuthResponeDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return new AuthResponeDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }
            else
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!passwordCheck)
                {
                    return new AuthResponeDto
                    {
                        Success = false,
                        Message = "Invalid password"
                    };
                }
            }
            //generate token
            var token = await GenerateJwtToken(user);
            return new AuthResponeDto
            {
                Success = true,
                Token = token.Token,
                RefreshToken = token.RefreshToken,
                UserId = user.Id,
                Expiration = token.Expiration,
                Roles = await _userManager.GetRolesAsync(user),
                Message = "Login successful"
            };
        }

        public async Task<AuthResponeDto> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var userExits = await _userManager.FindByEmailAsync(registerUserDto.Email);
            if (userExits != null)
            {
                return new AuthResponeDto
                {
                    Success = false,
                    Message = "User already exists"
                };
            }
            var user = new User()
            {
                UserName = registerUserDto.UserName,
                Email = registerUserDto.Email,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if(!result.Succeeded)
            {
                return new AuthResponeDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            //add role default 
            bool checkExitsRole = await _roleManager.RoleExistsAsync("Customer");
            if(!checkExitsRole)
            {
                var role = new IdentityRole<Guid>()
                {
                    Name = "Customer",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                await _roleManager.CreateAsync(role);
            }
            //add role to user 
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {
                return new AuthResponeDto
                {
                    Success = false,
                    Message = string.Join(", ", roleResult.Errors.Select(e => e.Description))
                };
            }
            //generate token
            var token = await GenerateJwtToken(user);
            return new AuthResponeDto
            {
                Success = true,
                Token = token.Token,
                RefreshToken = token.RefreshToken,
                UserId = user.Id,
                Expiration = token.Expiration,  
                Roles= await _userManager.GetRolesAsync(user),
                Message = result.Succeeded ? "User registered successfully" : "User registration failed"
            };
        }

        private async Task<AuthResponeDto> GenerateJwtToken(User user)
        {
            //get roles of user
            var userRoles = await _userManager.GetRolesAsync(user);
            //add claims of user, claims are key-value pairs that contain information about the user
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            };
            //add roles to claims
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            //get jwt settings from appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            //set key and signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                claims: authClaims,
                signingCredentials: cred
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();   
            return new AuthResponeDto
            {
                Success = true,
                Token = tokenString,
                RefreshToken = refreshToken,
            };
        }
    }
}
