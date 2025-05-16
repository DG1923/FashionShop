using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Models;

namespace FashionShop.CartService.Service.Interface
{
    public interface ICartItemService : IBaseService<CartItem, CartItemCreateDto, CartItemUpdateDto>
    {
    }
}
