using SMarket.Business.DTOs.Cart;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICustomMapper _mapper;
        private readonly IGenericRepository<Product> _productRepository;

        public CartService(ICartRepository cartRepository, ICustomMapper mapper, IGenericRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<CartSummaryDto> GetUserCartAsync(int userId)
        {
            var cartItems = await _cartRepository.GetUserCartItemsAsync(userId);
            var cartItemDtos = _mapper.Map<CartItem, CartItemDto>(cartItems);

            return new CartSummaryDto
            {
                Items = cartItemDtos.ToList()
            };
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            // Check if product is available and not hidden
            if (product.IsDeleted || product.IsHide || product.IsAdminHide)
            {
                throw new ArgumentException("Product is not available");
            }

            // Check stock availability
            if (product.StockQuantity < addToCartDto.Quantity)
            {
                throw new ArgumentException($"Insufficient stock. Available: {product.StockQuantity}, Requested: {addToCartDto.Quantity}");
            }

            // Check if item already exists in cart
            var existingCartItem = await _cartRepository.GetCartItemAsync(userId, addToCartDto.ProductId);

            if (existingCartItem != null)
            {
                // Update existing item quantity
                var newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;

                // Check stock for new total quantity
                if (product.StockQuantity < newQuantity)
                {
                    throw new ArgumentException($"Insufficient stock for total quantity. Available: {product.StockQuantity}, Total requested: {newQuantity}");
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.UnitPrice = (double)product.Price;

                var updatedCartItem = await _cartRepository.UpdateCartItemAsync(existingCartItem);
                return _mapper.Map<CartItem, CartItemDto>(updatedCartItem);
            }
            else
            {
                // Add new item to cart
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    UnitPrice = (double)product.Price
                };

                var addedCartItem = await _cartRepository.AddCartItemAsync(cartItem);
                return _mapper.Map<CartItem, CartItemDto>(addedCartItem);
            }
        }

        public async Task<CartItemDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto updateCartItemDto)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }

            var product = await _productRepository.GetByIdAsync(cartItem.ProductId!.Value);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            if (product.StockQuantity < updateCartItemDto.Quantity)
            {
                throw new ArgumentException($"Insufficient stock. Available: {product.StockQuantity}, Requested: {updateCartItemDto.Quantity}");
            }

            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.UnitPrice = (double)product.Price;

            var updatedCartItem = await _cartRepository.UpdateCartItemAsync(cartItem);
            return _mapper.Map<CartItem, CartItemDto>(updatedCartItem);
        }

        public async Task RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }

            await _cartRepository.RemoveCartItemAsync(cartItemId);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _cartRepository.ClearUserCartAsync(userId);
        }

        public async Task<int> GetCartItemsCountAsync(int userId)
        {
            return await _cartRepository.GetCartItemsCountAsync(userId);
        }

        public async Task<bool> IsProductInCartAsync(int userId, int productId)
        {
            return await _cartRepository.IsProductInCartAsync(userId, productId);
        }

        public async Task<CartItemDto?> GetCartItemAsync(int userId, int productId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);
            return cartItem != null ? _mapper.Map<CartItem, CartItemDto>(cartItem) : null;
        }
    }
}
