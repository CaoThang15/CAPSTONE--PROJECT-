using SMarket.DataAccess.Models;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetListFeedbacksAsync(ListFeedbackSearchCondition searchCondition);
        Task<int> GetCountFeedbacksAsync(ListFeedbackSearchCondition searchCondition);
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task CreateFeedbackAsync(Feedback feedback, SharedFile? sharedFile);
        Task UpdateFeedbackAsync(Feedback feedback, SharedFile? sharedFile);
        Task DeleteFeedbackAsync(int id);
    }
}
