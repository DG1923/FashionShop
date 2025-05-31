using FashionShop.CartService.DTO.CartItem;
using FashionShop.CartService.Models;
using FashionShop.CartService.Protos;
using FashionShop.CartService.Repo.Interface;
using FashionShop.CartService.Service.Interface;
using FashionShop.CartService.SyncDataService.Grpc;

namespace FashionShop.CartService.Service
{
    public class CartItemService : BaseService<CartItem, CartItemCreateDto, CartItemUpdateDto>, ICartItemService
    {
        private readonly IGenericRepo<CartItem> _repo;
        private readonly IGrpcCartItemClient _grpcClient;

        public CartItemService(IGenericRepo<CartItem> repo, IGrpcCartItemClient grpcClient) : base(repo)
        {
            _repo = repo;
            _grpcClient = grpcClient;
        }

        public async Task<IEnumerable<CartItemDisplayDto>> GetCartItemsByCartIdAsync(Guid cartId)
        {
            try
            {
                var cartItems = await _repo.GetAllAsync();
                if (cartItems == null)
                    return new List<CartItemDisplayDto>();

                var cartItemsInCart = cartItems.Where(ci => ci.CartId == cartId);
                foreach (var item in cartItemsInCart)
                {
                    GrpcGetQuantityRequest grpcGetQuantityRequest = new GrpcGetQuantityRequest
                    {
                        ProductId = item.ProductVariationId.ToString()
                    };
                    var grpcGetQuantityResponse = await _grpcClient.GetQuantityByProductId(grpcGetQuantityRequest);
                    if (grpcGetQuantityResponse == null || !grpcGetQuantityResponse.Result)
                    {
                        Console.WriteLine($"--> Failed to get quantity for product {item.ProductId}");
                        continue;
                    }
                    if (grpcGetQuantityResponse.Quantity < item.Quantity)
                    {
                        Console.WriteLine($"--> Insufficient quantity for product {item.ProductId}. Available: {grpcGetQuantityResponse.Quantity}, Requested: {item.Quantity}");
                        item.Quantity = grpcGetQuantityResponse.Quantity; // Adjust quantity to available stock
                    }
                    if (grpcGetQuantityResponse.Quantity <= 0)
                    {
                        Console.WriteLine($"--> Product {item.ProductId} is out of stock.");
                        item.Quantity = 0; // Set quantity to 0 if out of stock
                    }
                }
                return cartItemsInCart.Select(item => new CartItemDisplayDto
                {
                    Id = item.Id,
                    CartId = item.CartId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BasePrice = item.BasePrice,
                    DiscountPercent = item.DiscountPercent,
                    ProductColorId = item.ProductColorId,
                    ColorName = item.ColorName,
                    ColorCode = item.ColorCode,
                    ProductVariationId = item.ProductVariationId,
                    Size = item.Size,
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl,

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error getting cart items: {ex.Message}");
                throw;
            }
        }

        public override CartItem MapCreateToEntity(CartItemCreateDto dto)
        {
            return new CartItem
            {
                ProductId = dto.ProductId,
                CartId = dto.CartId,
                ProductName = dto.ProductName,
                BasePrice = dto.BasePrice,
                ProductColorId = dto.ProductColorId,
                ColorName = dto.ColorName,
                ColorCode = dto.ColorCode,
                ProductVariationId = dto.ProductVariationId,
                Size = dto.Size,
                InventoryId = dto.InventoryId,
                DiscountPercent = dto.DiscountPercent,
                Quantity = dto.Quantity,
                ImageUrl = dto.ImageUrl
            };
        }

        public override void MapToExistingEntity(CartItem entity, CartItemUpdateDto dto)
        {
            entity.ProductVariationId = dto.ProductVariationId;
            entity.ProductColorId = dto.ProductColorId;
            entity.ColorName = dto.ColorName;
            entity.ColorCode = dto.ColorCode;
            entity.Size = dto.Size;
            entity.InventoryId = dto.InventoryId;
            entity.Quantity = dto.Quantity;
            entity.CartId = dto.CartId;
            entity.ProductId = dto.ProductId;

        }

        public async Task<bool> RemoveCartItemByIdAsync(Guid cartItemId)
        {
            try
            {
                var cartItem = await _repo.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return false;

                // Return quantity to inventory through gRPC
                var updateRequest = new GrpcUpdateQuantityRequest
                {
                    ProductId = cartItem.ProductVariationId.ToString(),
                    Quantity = -cartItem.Quantity // Negative to return to inventory
                };

                var updateResponse = await _grpcClient.UpdateQuantity(updateRequest);
                if (!updateResponse.Result)
                {
                    Console.WriteLine($"--> Failed to return quantity to inventory for product {cartItem.ProductId}");
                    return false;
                }

                return await _repo.DeleteAsync(cartItemId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error removing cart item: {ex.Message}");
                return false;
            }
        }

        public async Task<CartItemUpdateDto> UpdateCartItemByIdAsync(Guid cartItemId, CartItemUpdateDto cartItemUpdateDto)
        {
            try
            {
                var existingItem = await _repo.GetByIdAsync(cartItemId);
                if (existingItem == null)
                    return null;

                // Calculate quantity difference
                var quantityDifference = cartItemUpdateDto.Quantity - existingItem.Quantity;

                if (quantityDifference != 0)
                {
                    // Check inventory through gRPC
                    var quantityRequest = new GrpcGetQuantityRequest
                    {
                        ProductId = existingItem.ProductVariationId.ToString()
                    };

                    var quantityResponse = await _grpcClient.GetQuantityByProductId(quantityRequest);
                    if (!quantityResponse.Result || quantityResponse.Quantity < quantityDifference)
                    {
                        Console.WriteLine($"--> Insufficient quantity. Available: {quantityResponse.Quantity}");
                        return null;
                    }

                    // Update inventory through gRPC
                    var updateRequest = new GrpcUpdateQuantityRequest
                    {
                        ProductId = existingItem.ProductVariationId.ToString(),
                        Quantity = quantityDifference
                    };

                    var updateResponse = await _grpcClient.UpdateQuantity(updateRequest);
                    if (!updateResponse.Result)
                    {
                        Console.WriteLine($"--> Failed to update inventory quantity");
                        return null;
                    }
                }

                // Update cart item
                MapToExistingEntity(existingItem, cartItemUpdateDto);
                var success = await _repo.UpdateAsync(existingItem);

                return success ? cartItemUpdateDto : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error updating cart item: {ex.Message}");
                return null;
            }
        }
    }
}
