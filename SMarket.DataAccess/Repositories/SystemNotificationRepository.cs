using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.Utility.Enums;

namespace SMarket.DataAccess.Repositories
{
    public class SystemNotificationRepository : ISystemNotificationRepository
    {
        private readonly AppDbContext _context;

        public SystemNotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SystemNotification> CreateSystemNotificationAsync(SystemNotification systemNotification)
        {
            _context.SystemNotifications.Add(systemNotification);
            await _context.SaveChangesAsync();
            return systemNotification;
        }

        public async Task<IEnumerable<SystemNotification>> GetSystemNotificationsAsync(int page, int pageSize)
        {
            return await _context.SystemNotifications
                .OrderByDescending(sn => sn.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SystemNotification?> GetSystemNotificationByIdAsync(int id)
        {
            return await _context.SystemNotifications
                .FirstOrDefaultAsync(sn => sn.Id == id);
        }

        public async Task<IEnumerable<SystemNotification>> GetPendingScheduledNotificationsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.SystemNotifications
                .Where(sn => !sn.IsSent && sn.TimeToSend <= now)
                .OrderBy(sn => sn.TimeToSend)
                .ToListAsync();
        }

        public async Task<bool> MarkAsSentAsync(int systemNotificationId)
        {
            var notification = await _context.SystemNotifications
                .FirstOrDefaultAsync(sn => sn.Id == systemNotificationId);

            if (notification == null)
                return false;

            notification.IsSent = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSystemNotificationAsync(int id)
        {
            var notification = await _context.SystemNotifications
                .FirstOrDefaultAsync(sn => sn.Id == id);

            if (notification == null)
                return false;

            _context.SystemNotifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.SystemNotifications.CountAsync();
        }

        public async Task<List<int>> GetAllActiveUserIdsAsync()
        {
            return await _context.Users
                .Where(u => u.RoleId != (int)RoleType.Admin && !u.IsDeleted)
                .Select(u => u.Id)
                .ToListAsync();
        }
    }
}
