using Microsoft.Extensions.DependencyInjection;
using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Notification;
using SMarket.Business.DTOs.SystemNotification;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class SystemNotificationService : ISystemNotificationService
    {
        private readonly ISystemNotificationRepository _systemNotificationRepository;
        private readonly INotificationService _notificationService;
        private readonly ICustomMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SystemNotificationService(
            ISystemNotificationRepository systemNotificationRepository,
            INotificationService notificationService,
            ICustomMapper mapper,
            IServiceScopeFactory scopeFactory)
        {
            _systemNotificationRepository = systemNotificationRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }

        public async Task<SystemNotificationDto> CreateSystemNotificationAsync(CreateSystemNotificationDto createDto, int adminUserId)
        {
            var systemNotification = new SystemNotification
            {
                Content = createDto.Content,
                Type = createDto.Type,
                TimeToSend = createDto.IsImmediate ? DateTime.UtcNow : createDto.TimeToSend,
                IsSent = false,
                IdRefer = createDto.IdRefer,
                CreateByUser = adminUserId,
            };

            var created = await _systemNotificationRepository.CreateSystemNotificationAsync(systemNotification);

            // If immediate, send right away
            if (createDto.IsImmediate)
            {
                await SendSystemNotificationNowAsync(created.Id);
            }

            return _mapper.Map<SystemNotification, SystemNotificationDto>(created);
        }

        public async Task<PaginationResult<SystemNotificationDto>> GetSystemNotificationsAsync(int page = 1, int pageSize = 20)
        {
            var notifications = await _systemNotificationRepository.GetSystemNotificationsAsync(page, pageSize);
            var notificationDtos = _mapper.Map<SystemNotification, SystemNotificationDto>(notifications);

            return new PaginationResult<SystemNotificationDto>(
                currentPage: page,
                pageSize: pageSize,
                totalItems: notificationDtos.Count(),
                items: notificationDtos
            );
        }

        public async Task<SystemNotificationDto?> GetSystemNotificationByIdAsync(int id)
        {
            var notification = await _systemNotificationRepository.GetSystemNotificationByIdAsync(id);
            return notification != null ? _mapper.Map<SystemNotification, SystemNotificationDto>(notification) : null;
        }

        public async Task<bool> DeleteSystemNotificationAsync(int id)
        {
            return await _systemNotificationRepository.DeleteSystemNotificationAsync(id);
        }

        public async Task ProcessScheduledNotificationsAsync()
        {
            var pendingNotifications = await _systemNotificationRepository.GetPendingScheduledNotificationsAsync();

            foreach (var systemNotification in pendingNotifications)
            {
                try
                {
                    await SendSystemNotificationNowAsync(systemNotification.Id);
                }
                catch (Exception)
                {

                }
            }
        }

        public async Task SendSystemNotificationNowAsync(int systemNotificationId)
        {
            var systemNotification = await _systemNotificationRepository.GetSystemNotificationByIdAsync(systemNotificationId);
            if (systemNotification == null || systemNotification.IsSent)
                return;

            List<int> targetUserIds = await _systemNotificationRepository.GetAllActiveUserIdsAsync();

            await _notificationService.BroadcastNotification(
                userIds: targetUserIds,
                content: systemNotification.Content,
                type: systemNotification.Type,
                referenceId: systemNotification.IdRefer
            );

            await _systemNotificationRepository.MarkAsSentAsync(systemNotificationId);
        }
    }
}
