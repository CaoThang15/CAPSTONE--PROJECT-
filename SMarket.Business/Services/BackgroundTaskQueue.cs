using System.Collections.Concurrent;
using SMarket.Business.Services.Interfaces;

namespace SMarket.Business.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Action> _workItems = new();
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _asyncWorkItems = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly SemaphoreSlim _asyncSignal = new(0);

        public void QueueOtp(Action workItem)
        {
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public void QueueWorkItem(Func<CancellationToken, Task> workItem)
        {
            _asyncWorkItems.Enqueue(workItem);
            _asyncSignal.Release();
        }

        public async Task<Action> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem!;
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsyncWorkItem(CancellationToken cancellationToken)
        {
            await _asyncSignal.WaitAsync(cancellationToken);
            _asyncWorkItems.TryDequeue(out var workItem);
            return workItem!;
        }
    }
}