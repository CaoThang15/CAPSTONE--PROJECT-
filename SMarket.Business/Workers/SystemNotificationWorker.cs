using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMarket.Business.Services.Interfaces;

namespace SMarket.Business.Workers
{
    public class SystemNotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SystemNotificationWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

        public SystemNotificationWorker(
            IServiceProvider serviceProvider,
            ILogger<SystemNotificationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SystemNotificationWorker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var systemNotificationService = scope.ServiceProvider.GetRequiredService<ISystemNotificationService>();

                    await systemNotificationService.ProcessScheduledNotificationsAsync();

                    _logger.LogDebug("Processed scheduled notifications");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing scheduled notifications");
                }

                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("SystemNotificationWorker stopped");
        }
    }
}
