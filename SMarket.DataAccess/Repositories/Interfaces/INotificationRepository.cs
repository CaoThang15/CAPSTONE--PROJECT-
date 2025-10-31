using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<PersonalNotification>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10);
        Task<PersonalNotification?> GetNotificationByIdAsync(int notificationId);
        Task<PersonalNotification> CreateNotificationAsync(PersonalNotification notification);
        Task<PersonalNotification> UpdateNotificationAsync(PersonalNotification notification);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<int> GetCountNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
