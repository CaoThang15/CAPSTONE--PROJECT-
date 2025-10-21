using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Cart;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<Response>> GetCart()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                var cart = await _cartService.GetUserCartAsync(userId);

                return Ok(new Response
                {
                    Message = "Cart retrieved successfully.",
                    Data = cart
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response>> AddToCart(AddToCartDto addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid cart data.",
                        Data = errors
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);

                return Ok(new Response
                {
                    Message = "Item added to cart successfully.",
                    Data = cartItem
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to add item to cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch]
        public async Task<ActionResult<Response>> UpdateCartItem(UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid cart item data.",
                        Data = errors
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                var cartItem = await _cartService.UpdateCartItemAsync(userId, updateCartItemDto.CartItemId, updateCartItemDto);

                return Ok(new Response
                {
                    Message = "Cart item updated successfully.",
                    Data = cartItem
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response
                {
                    Message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update cart item.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{cartItemId}")]
        public async Task<ActionResult<Response>> RemoveFromCart(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                await _cartService.RemoveFromCartAsync(userId, cartItemId);

                return Ok(new Response
                {
                    Message = "Item removed from cart successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response
                {
                    Message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to remove item from cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<Response>> ClearCart()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                await _cartService.ClearCartAsync(userId);

                return Ok(new Response
                {
                    Message = "Cart cleared successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to clear cart.",
                    Data = ex.Message
                });
            }
        }
    }
}
