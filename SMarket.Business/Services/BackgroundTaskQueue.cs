using System.Collections.Concurrent;
using SMarket.Business.Services.Interfaces;

namespace SMarket.Business.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Action> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void QueueOtp(Action workItem)
        {
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Action> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem!;
        }
    }
}