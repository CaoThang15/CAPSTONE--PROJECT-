namespace SMarket.Business.Services.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueOtp(Action workItem);
        Task<Action> DequeueAsync(CancellationToken cancellationToken);
    }
}