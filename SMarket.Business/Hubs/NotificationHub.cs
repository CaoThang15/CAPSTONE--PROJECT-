using Microsoft.AspNetCore.Http;
using SMarket.Business.DTOs.Notification;
using SMarket.Business.Hubs;
using System.Collections.Concurrent;
using System.Text.Json;

namespace SMarket.Business.Services
{
    public class NotificationHub : INotificationHub
    {
        private readonly ConcurrentDictionary<int, List<SseConnection>> _userConnections = new();

        public async Task SendNotificationToUser(int userId, NotificationDto notification)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                var data = JsonSerializer.Serialize(new
                {
                    type = "notification",
                    data = notification
                });

                var message = $"data: {data}\n\n";
                var tasks = connections.Where(c => !c.Response.HttpContext.RequestAborted.IsCancellationRequested)
                                     .Select(c => WriteToStreamAsync(c.Response, message));

                await Task.WhenAll(tasks);

                _userConnections[userId] = connections
                    .Where(c => !c.Response.HttpContext.RequestAborted.IsCancellationRequested)
                    .ToList();

                if (!_userConnections[userId].Any())
                {
                    _userConnections.TryRemove(userId, out _);
                }
            }
        }

        public void AddConnection(int userId, HttpResponse response)
        {
            var connection = new SseConnection { Response = response };

            _userConnections.AddOrUpdate(userId,
                new List<SseConnection> { connection },
                (key, existing) =>
                {
                    existing.Add(connection);
                    return existing;
                });
        }

        public void RemoveConnection(int userId, HttpResponse response)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.RemoveAll(c => c.Response == response);

                if (!connections.Any())
                {
                    _userConnections.TryRemove(userId, out _);
                }
            }
        }

        private static async Task WriteToStreamAsync(HttpResponse response, string message)
        {
            try
            {
                await response.WriteAsync(message);
                await response.Body.FlushAsync();
            }
            catch
            {
            }
        }

        private class SseConnection
        {
            public HttpResponse Response { get; set; } = null!;
        }
    }
}
