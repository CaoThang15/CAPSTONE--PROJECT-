using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.SystemNotification;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using SMarket.Utility.Enums;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [Route("api/system-notification")]
    [ApiController]
    [Authorize(Roles = nameof(RoleType.Admin))]
    public class SystemNotificationController : ControllerBase
    {
        private readonly ISystemNotificationService _systemNotificationService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public SystemNotificationController(
            ISystemNotificationService systemNotificationService,
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _systemNotificationService = systemNotificationService;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        // ADMIN ENDPOINTS FOR SYSTEM NOTIFICATIONS

        /// <summary>
        /// Create a system notification (Admin only)
        /// </summary>
        [HttpPost("system")]
        public async Task<IActionResult> CreateSystemNotification(CreateSystemNotificationDto createDto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var systemNotification = await _systemNotificationService.CreateSystemNotificationAsync(createDto, userId.Value);

            return Ok(new Response
            {
                Message = "System notification created successfully",
                Data = systemNotification
            });
        }

        /// <summary>
        /// Get all system notifications (Admin only)
        /// </summary>
        [HttpGet("system")]
        public async Task<IActionResult> GetSystemNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _systemNotificationService.GetSystemNotificationsAsync(page, pageSize);

            return Ok(new Response
            {
                Message = "System notifications retrieved successfully",
                Data = result
            });
        }

        /// <summary>
        /// Get system notification by ID (Admin only)
        /// </summary>
        [HttpGet("system/{id}")]
        public async Task<IActionResult> GetSystemNotificationById(int id)
        {
            var notification = await _systemNotificationService.GetSystemNotificationByIdAsync(id);

            if (notification == null)
                return NotFound(new Response
                {
                    Message = "System notification not found"
                });

            return Ok(new Response
            {
                Message = "System notification retrieved successfully",
                Data = notification
            });
        }

        /// <summary>
        /// Send system notification immediately (Admin only)
        /// </summary>
        [HttpPost("system/{id}/send-now")]
        public async Task<IActionResult> SendSystemNotificationNow(int id)
        {
            // Queue the task for background processing
            _backgroundTaskQueue.QueueWorkItem(async cancellationToken =>
            {
                try
                {
                    await _systemNotificationService.SendSystemNotificationNowAsync(id);
                }
                catch (Exception)
                {

                }
            });

            return Ok(new Response
            {
                Message = "System notification has been queued for sending"
            });
        }

        /// <summary>
        /// Delete system notification (Admin only)
        /// </summary>
        [HttpDelete("system/{id}")]
        public async Task<IActionResult> DeleteSystemNotification(int id)
        {
            var result = await _systemNotificationService.DeleteSystemNotificationAsync(id);

            if (!result)
                return NotFound(new Response
                {
                    Message = "System notification not found"
                });

            return Ok(new Response
            {
                Message = "System notification deleted successfully"
            });
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
    }
}
