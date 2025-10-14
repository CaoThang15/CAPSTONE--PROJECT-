using Microsoft.Extensions.Hosting;
using SMarket.Business.Services.Interfaces;

namespace SMarket.Business.Services.Workers
{
    public class OtpWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public OtpWorker(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    workItem();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending OTP: {ex.Message}");
                }
            }
        }
    }
}