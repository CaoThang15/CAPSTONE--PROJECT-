using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto?> GetCategoryBySlugAsync(string slug);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int cateId, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(int id);
        Task<string> GetUniqueSlug(long id, string name);
    }
}
