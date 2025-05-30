using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service.Interface;

namespace FashionShop.CartService.Service
{
    public class CartService : BaseService<Cart, CartCreateDto, CartUpdateDto>, ICartService
    {
        public CartService(IGenericRepo<Cart> repo) : base(repo)
        {
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
