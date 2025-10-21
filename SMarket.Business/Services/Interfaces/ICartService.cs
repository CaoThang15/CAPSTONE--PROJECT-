using SMarket.Business.DTOs.Cart;

namespace SMarket.Business.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartSummaryDto> GetUserCartAsync(int userId);
        Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartItemDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto updateCartItemDto);
        Task RemoveFromCartAsync(int userId, int cartItemId);
        Task ClearCartAsync(int userId);
        Task<int> GetCartItemsCountAsync(int userId);
        Task<bool> IsProductInCartAsync(int userId, int productId);
        Task<CartItemDto?> GetCartItemAsync(int userId, int productId);
    }
}
