using System.ComponentModel.DataAnnotations;

namespace FashionShop.UserService.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
