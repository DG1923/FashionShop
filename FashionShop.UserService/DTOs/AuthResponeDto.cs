namespace FashionShop.UserService.DTOs
{
    public class AuthResponeDto
    {
       
            public bool Success { get; set; }
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public DateTime Expiration { get; set; }
            public string Message { get; set; }
            public Guid UserId { get; set; }
            public IList<string> Roles { get; set; } = new List<string>();
        
    }
}
