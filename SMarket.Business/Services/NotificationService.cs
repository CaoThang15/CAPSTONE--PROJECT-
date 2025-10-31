using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Notification;
using SMarket.Business.Hubs;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.Utility.Enums;

namespace SMarket.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICustomMapper _mapper;
        private readonly INotificationHub _notificationHub;

        public NotificationService(
            INotificationRepository notificationRepository,
            ICustomMapper mapper,
            INotificationHub notificationHub)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _notificationHub = notificationHub;
        }

        public async Task<PaginationResult<NotificationDto>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, page, pageSize);
            var notificationDtos = _mapper.Map<PersonalNotification, NotificationDto>(notifications);
            var total = await _notificationRepository.GetCountNotificationsAsync(userId);
            return new PaginationResult<NotificationDto>(
                currentPage: page,
                pageSize: pageSize,
                totalItems: total,
                items: notificationDtos
            );
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = new PersonalNotification
            {
                Type = createNotificationDto.Type,
                Content = createNotificationDto.Content,
                ToUserId = createNotificationDto.ToUserId,
                IdRefer = createNotificationDto.IdRefer,
                SendAt = createNotificationDto.SendAt ?? DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateNotificationAsync(notification);
            var notificationDto = _mapper.Map<PersonalNotification, NotificationDto>(createdNotification);

            await _notificationHub.SendNotificationToUser(notification.ToUserId, notificationDto);

            return notificationDto;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var result = await _notificationRepository.MarkAsReadAsync(notificationId, userId);

            return result;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var result = await _notificationRepository.MarkAllAsReadAsync(userId);

            return result;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var result = await _notificationRepository.DeleteNotificationAsync(notificationId, userId);

            return result;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task SendNotificationToUserAsync(int userId, string content, int type, int? idRefer = null)
        {
            var createDto = new CreateNotificationDto
            {
                Type = type,
                Content = content,
                ToUserId = userId,
                IdRefer = idRefer,
                SendAt = DateTime.UtcNow
            };

            await CreateNotificationAsync(createDto);
        }

        public async Task NotifyOrderPlaced(int userId, int orderId)
        {
            var content = $"Your order #{orderId} has been placed successfully.";
            await SendNotificationToUserAsync(
                userId,
                content,
                (int)NotificationType.Order,
                orderId
            );
        }

        public async Task NotifyOrderStatusChanged(int userId, int orderId, string newStatus)
        {
            var content = $"Your order #{orderId} status has been updated to '{newStatus}'";
            await SendNotificationToUserAsync(
                userId,
                content,
                (int)NotificationType.Order,
                orderId
            );
        }

        public async Task NotifyVoucherAssigned(int userId, int voucherId, string voucherCode)
        {
            var content = $"New voucher '{voucherCode}' has been assigned to your account!";
            await SendNotificationToUserAsync(
                userId,
                content,
                (int)NotificationType.Voucher,
                voucherId
            );
        }

        public async Task NotifyVoucherExpiring(int userId, int voucherId, string voucherCode, DateTime expiryDate)
        {
            var daysLeft = (expiryDate.Date - DateTime.UtcNow.Date).Days;
            var content = $"Your voucher '{voucherCode}' will expire in {daysLeft} day(s). Use it before {expiryDate:MMM dd, yyyy}!";
            await SendNotificationToUserAsync(
                userId,
                content,
                (int)NotificationType.Voucher,
                voucherId
            );
        }

        public async Task NotifyGeneral(int userId, string content, int? referenceId = null)
        {
            await SendNotificationToUserAsync(
                userId,
                content,
                (int)NotificationType.General,
                referenceId
            );
        }

        public async Task BroadcastNotification(IEnumerable<int> userIds, string content, int type, int? referenceId = null)
        {
            var tasks = userIds.Select(userId =>
                SendNotificationToUserAsync(userId, content, type, referenceId)
            );

            await Task.WhenAll(tasks);
        }
    }
}
