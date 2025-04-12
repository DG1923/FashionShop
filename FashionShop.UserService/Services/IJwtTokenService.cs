using FashionShop.UserService.Models;
using System.Security.Claims;

namespace FashionShop.UserService.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
