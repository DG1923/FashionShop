using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Models;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service.Interface;

namespace FashionShop.CartService.Service
{
    public class CartItemService : BaseService<CartItem, CartItemCreateDto, CartItemUpdateDto>, ICartItemService
    {
        public CartItemService(IGenericRepo<CartItem> repo) : base(repo)
        {

        }
        public override CartItem MapCreateToEntity(CartItemCreateDto dto)
        {
            return new CartItem
            {
                ImageUrl = dto.ImageUrl,
                Price = dto.Price,
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,


            };
        }
        public override void MapToExistingEntity(CartItem entity, CartItemUpdateDto dto)
        {
            entity.ImageUrl = dto.ImageUrl;
            entity.Price = dto.Price;
            entity.ProductId = dto.ProductId;
            entity.ProductName = dto.ProductName;
            entity.Quantity = dto.Quantity;

        }
    }
}
