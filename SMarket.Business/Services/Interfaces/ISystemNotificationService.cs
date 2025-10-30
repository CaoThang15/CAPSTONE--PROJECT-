using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.SystemNotification;

namespace SMarket.Business.Services.Interfaces
{
    public interface ISystemNotificationService
    {
        Task<SystemNotificationDto> CreateSystemNotificationAsync(CreateSystemNotificationDto createDto, int adminUserId);
        Task<PaginationResult<SystemNotificationDto>> GetSystemNotificationsAsync(int page = 1, int pageSize = 20);
        Task<SystemNotificationDto?> GetSystemNotificationByIdAsync(int id);
        Task<bool> DeleteSystemNotificationAsync(int id);
        Task ProcessScheduledNotificationsAsync();
        Task SendSystemNotificationNowAsync(int systemNotificationId);
    }
}
