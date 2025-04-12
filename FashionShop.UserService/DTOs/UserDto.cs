using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FashionShop.UserService.DTOs
{
    public class UserDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public IList<IdentityRole<Guid>> Roles { get; set; }
    }
}
