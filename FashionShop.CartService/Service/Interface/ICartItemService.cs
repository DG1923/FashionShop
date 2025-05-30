using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Models;

namespace FashionShop.CartService.Service.Interface
{
    public interface ICartItemService : IBaseService<CartItem, CartItemCreateDto, CartItemUpdateDto>
    {
        Task<IEnumerable<CartItemDisplayDto>> GetCartItemsByCartIdAsync(Guid cartId);
        Task<bool> RemoveCartItemByIdAsync(Guid cartId);
        Task<CartItemUpdateDto> UpdateCartItemByIdAsync(Guid cartId, CartItemUpdateDto cartItemUpdateDto);


    }

}
