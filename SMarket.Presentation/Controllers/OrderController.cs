using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Order;
using SMarket.Business.Enums;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility;
using SMarket.Utility.Enums;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVoucherService _voucherService;
        private readonly INotificationService _notificationService;

        public OrderController(IOrderService orderService, IVoucherService voucherService, INotificationService notificationService)
        {
            _orderService = orderService;
            _voucherService = voucherService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Truyền vào Order: desc, asc.
        /// Truyền vào OrderBy: Created_At, Name, Price.
        /// Truyền vào Page &gt;= 1.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.Seller))]
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

                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (!Enum.TryParse(userRoleClaim, out RoleType userRole))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user role."
                    });
                }

                var orders = await _orderService.GetListOrdersAsync(userId, userRole, searchCondition);

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

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<Response>> GetMyOrders([FromQuery] ListOrderSearchCondition condition)
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
                var result = await _orderService.GetBuyerOrdersAsync(userId, condition);

                return Ok(new Response
                {
                    Message = "Buyer orders retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve buyer orders.",
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

                await _voucherService.ApplyVoucherAsync(createDto.VoucherId);

                var listNewOrders = await _orderService.CreateOrderAsync(createDto, userId);

                foreach (var newOrder in listNewOrders)
                {
                    await _notificationService.NotifyOrderPlaced(
                        userId,
                        newOrder.Id
                    );
                }

                return Ok(new Response
                {
                    Message = "Order created successfully.",
                    Data = listNewOrders
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

                var order = await _orderService.UpdateOrderStatusAsync(id, status);

                await _notificationService.NotifyOrderStatusChanged(
                    order.CustomerId,
                    order.Id,
                    order.StatusName
                );

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
