using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.UserService.DTOs
{
    public class UpdateUserRoleDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public List<IdentityRole<Guid>> Roles { get; set; }
    }
}
