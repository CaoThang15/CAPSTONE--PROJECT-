using SMarket.Business.DTOs.AI;

namespace SMarket.Business.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GenerateProductDescriptionAsync(ProductSuggestionInput input);
    }
}
