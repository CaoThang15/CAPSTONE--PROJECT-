using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.DataAccess.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonalNotification>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10)
        {
            return await _context.PersonalNotifications
                .Where(n => n.ToUserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PersonalNotification?> GetNotificationByIdAsync(int notificationId)
        {
            return await _context.PersonalNotifications
                .Where(n => n.Id == notificationId && !n.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<PersonalNotification> CreateNotificationAsync(PersonalNotification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.SendAt ??= DateTime.UtcNow;

            _context.PersonalNotifications.Add(notification);
            await _context.SaveChangesAsync();

            return await GetNotificationByIdAsync(notification.Id) ?? notification;
        }

        public async Task<PersonalNotification> UpdateNotificationAsync(PersonalNotification notification)
        {
            notification.UpdatedAt = DateTime.UtcNow;

            _context.PersonalNotifications.Update(notification);
            await _context.SaveChangesAsync();

            return await GetNotificationByIdAsync(notification.Id) ?? notification;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.PersonalNotifications
                .Where(n => n.Id == notificationId && n.ToUserId == userId && !n.IsDeleted)
                .FirstOrDefaultAsync();

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _context.PersonalNotifications
                .Where(n => n.ToUserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return unreadNotifications.Any();
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.PersonalNotifications
                .Where(n => n.Id == notificationId && n.ToUserId == userId && !n.IsDeleted)
                .FirstOrDefaultAsync();

            if (notification == null)
                return false;

            notification.IsDeleted = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCountNotificationsAsync(int userId)
        {
            return await _context.PersonalNotifications
                .CountAsync(n => n.ToUserId == userId && !n.IsDeleted);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.PersonalNotifications
                .CountAsync(n => n.ToUserId == userId && !n.IsRead && !n.IsDeleted);
        }
    }
}
