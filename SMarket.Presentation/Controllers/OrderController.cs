using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Order;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Truyền vào Order: desc, asc.
        /// Truyền vào OrderBy: Created_At, Name, Price.
        /// Truyền vào Page &gt;= 1.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Response>> SearchOrders([FromQuery] ListOrderSearchCondition searchCondition)
        {
            try
            {
                var orders = await _orderService.GetListOrdersAsync(searchCondition);

                return Ok(new Response
                {
                    Message = "Orders retrieved successfully.",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve orders.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<Response>> SearchOrdersOfUser([FromQuery] ListOrderSearchCondition searchCondition)
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

                searchCondition.UserId = userId;
                var orders = await _orderService.GetListOrdersAsync(searchCondition);

                return Ok(new Response
                {
                    Message = "Orders retrieved successfully.",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve orders.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Order not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Order retrieved successfully.",
                    Data = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve order.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<Response>> GetListOrderStatuses()
        {
            try
            {
                var statuses = await _orderService.GetListOrderStatusesAsync();

                return Ok(new Response
                {
                    Message = "Order status retrieved successfully.",
                    Data = statuses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve order status.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(CreateOrUpdateOrder createDto)
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
                        Message = "Invalid order data.",
                        Data = errors
                    });
                }

                await _orderService.CreateOrderAsync(createDto, userId);

                return Ok(new Response
                {
                    Message = "Order created successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to create order.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Response>> UpdateOrder(int id, CreateOrUpdateOrder updateDto)
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
                        Message = "Invalid order data.",
                        Data = errors
                    });
                }

                await _orderService.UpdateOrderAsync(id, updateDto);

                return Ok(new Response
                {
                    Message = "Order updated successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update order.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<Response>> UpdateOrderStatus(int id, int status)
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
                        Message = "Invalid order data.",
                        Data = errors
                    });
                }

                await _orderService.UpdateOrderStatusAsync(id, status);

                return Ok(new Response
                {
                    Message = "Order updated successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update order.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);

                return Ok(new Response
                {
                    Message = "Order deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to delete order.",
                    Data = ex.Message
                });
            }
        }
    }
}
