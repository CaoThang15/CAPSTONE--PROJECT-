using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Notification;
using SMarket.Business.DTOs.SystemNotification;
using SMarket.Business.Hubs;
using SMarket.Business.Services;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationHub _notificationHub;

        public NotificationController(
            INotificationService notificationService,
            INotificationHub notificationHub)
        {
            _notificationService = notificationService;
            _notificationHub = notificationHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value, page, pageSize);

            return Ok(new Response
            {
                Message = "Notifications retrieved successfully",
                Data = notifications
            });
        }

        [HttpPatch("mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var result = await _notificationService.MarkAsReadAsync(id, userId.Value);

            if (!result)
            {
                return NotFound(new Response
                {
                    Message = "Notification not found or access denied",
                });
            }

            return Ok(new Response
            {
                Message = "Notification marked as read",
            });
        }

        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId.Value);

            return Ok(new Response
            {
                Message = "All notifications marked as read",
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var result = await _notificationService.DeleteNotificationAsync(id, userId.Value);

            if (!result)
            {
                return NotFound(new Response
                {
                    Message = "Notification not found or access denied"
                });
            }

            return Ok(new Response
            {
                Message = "Notification deleted successfully"
            });
        }

        [HttpGet("stream")]
        public async Task<IActionResult> NotificationStream()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("Access-Control-Allow-Origin", "*");

            _notificationHub.AddConnection(userId.Value, Response);

            try
            {
                await Response.WriteAsync("data: {\"type\":\"connected\",\"data\":{\"message\":\"Connection established\"}}\n\n");
                await Response.Body.FlushAsync();

                var cancellationToken = HttpContext.RequestAborted;
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception)
            {

            }
            finally
            {
                _notificationHub.RemoveConnection(userId.Value, Response);
            }

            return new EmptyResult();
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            await _notificationService.SendNotificationToUserAsync(
                userId.Value,
                "Test notification",
                1,
                null
            );

            return Ok(new Response
            {
                Message = "Test notification sent"
            });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var unreadCount = await _notificationService.GetUnreadCountAsync(userId.Value);

            return Ok(new Response
            {
                Message = "Unread count retrieved successfully",
                Data = new { unreadCount }
            });
        }
    }
}
