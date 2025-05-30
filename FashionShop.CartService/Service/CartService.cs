using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service.Interface;

namespace FashionShop.CartService.Service
{
    public class CartService : BaseService<Cart, CartCreateDto, CartUpdateDto>, ICartService
    {
        private readonly IGenericRepo<Cart> _repo;
        public CartService(IGenericRepo<Cart> repo) : base(repo)
        {
            _repo = repo;
        }

        public async Task<Guid> GetCartIdByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var cart = await _repo.GetAllAsync();
            if (cart == null || !cart.Any())
            {
                return Guid.Empty; // No carts found
            }
            var cartId = cart.FirstOrDefault(c => c.UserId == userId)?.Id;
            if (cartId == null)
            {
                return Guid.Empty; // No cart found for the user
            }
            return cartId.Value; // Return the found cart ID
        }

        public override Cart MapCreateToEntity(CartCreateDto dto)
        {
            return new Cart
            {
                UserId = dto.UserId

            };

        }
        public override void MapToExistingEntity(Cart entity, CartUpdateDto dto)
        {
            entity.UserId = dto.UserId;
            entity.UpdatedAt = DateTime.UtcNow;

        }
    }
}
