using Microsoft.AspNetCore.Http;
using SMarket.Business.DTOs.Notification;

namespace SMarket.Business.Hubs
{
    public interface INotificationHub
    {
        Task SendNotificationToUser(int userId, NotificationDto notification);
        void AddConnection(int userId, HttpResponse response);
        void RemoveConnection(int userId, HttpResponse response);
    }
}