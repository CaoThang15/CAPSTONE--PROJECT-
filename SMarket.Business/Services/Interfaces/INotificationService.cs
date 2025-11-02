using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Notification;

namespace SMarket.Business.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PaginationResult<NotificationDto>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task SendNotificationToUserAsync(int userId, string content, int type, int? idRefer = null);
        Task NotifyOrderPlaced(int userId, int orderId);
        Task NotifyOrderStatusChanged(int userId, int orderId, string newStatus);
        Task NotifyVoucherAssigned(int userId, int voucherId, string voucherCode);
        Task NotifyGeneral(int userId, string content, int? referenceId = null);
        Task BroadcastNotification(IEnumerable<int> userIds, string content, int type, int? referenceId = null);
    }
}
