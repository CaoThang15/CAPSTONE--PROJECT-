using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.DataAccess.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetUserCartItemsAsync(int userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId && c.ProductId == productId && !c.IsDeleted)
                .Include(c => c.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Where(c => c.Id == cartItemId && !c.IsDeleted)
                .Include(c => c.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            cartItem.CreatedAt = DateTime.UtcNow;

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return await GetCartItemByIdAsync(cartItem.Id) ?? cartItem;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItem cartItem)
        {
            cartItem.UpdatedAt = DateTime.UtcNow;

            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return await GetCartItemByIdAsync(cartItem.Id) ?? cartItem;
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.IsDeleted = true;
                cartItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearUserCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemsCountAsync(int userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .SumAsync(c => c.Quantity);
        }

        public async Task<bool> IsProductInCartAsync(int userId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(c => c.UserId == userId && c.ProductId == productId && !c.IsDeleted);
        }
    }
}
