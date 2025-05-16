using FashionShop.CartService.DTO.Cart;
using FashionShop.CartService.Models;

namespace FashionShop.CartService.Service.Interface
{
    public interface ICartService : IBaseService<Cart, CartCreateDto, CartUpdateDto>
    {
    }
}
