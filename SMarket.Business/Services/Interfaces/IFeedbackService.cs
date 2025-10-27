using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Feedback;
using SMarket.Business.DTOs.Product;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<PaginationResult<FeedbackDto>> GetListFeedbacksAsync(ListFeedbackSearchCondition searchCondition);
        Task<FeedbackDto> GetFeedbackByIdAsync(int id);
        Task CreateFeedbackAsync(CreateOrUpdateFeedbackDto feedback);
        Task UpdateFeedbackAsync(int id, CreateOrUpdateFeedbackDto feedback);
        Task DeleteFeedbackAsync(int id);
    }
}
