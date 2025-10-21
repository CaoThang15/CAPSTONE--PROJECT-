using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetUserCartItemsAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearUserCartAsync(int userId);
        Task<int> GetCartItemsCountAsync(int userId);
        Task<bool> IsProductInCartAsync(int userId, int productId);
    }
}
