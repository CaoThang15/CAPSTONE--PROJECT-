using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface ISystemNotificationRepository
    {
        Task<SystemNotification> CreateSystemNotificationAsync(SystemNotification systemNotification);
        Task<IEnumerable<SystemNotification>> GetSystemNotificationsAsync(int page, int pageSize);
        Task<SystemNotification?> GetSystemNotificationByIdAsync(int id);
        Task<IEnumerable<SystemNotification>> GetPendingScheduledNotificationsAsync();
        Task<bool> MarkAsSentAsync(int systemNotificationId);
        Task<bool> DeleteSystemNotificationAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<List<int>> GetAllActiveUserIdsAsync();
    }
}
