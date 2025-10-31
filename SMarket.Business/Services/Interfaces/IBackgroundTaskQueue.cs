namespace SMarket.Business.Services.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueOtp(Action workItem);
        void QueueWorkItem(Func<CancellationToken, Task> workItem);
        Task<Action> DequeueAsync(CancellationToken cancellationToken);
        Task<Func<CancellationToken, Task>> DequeueAsyncWorkItem(CancellationToken cancellationToken);
    }
}