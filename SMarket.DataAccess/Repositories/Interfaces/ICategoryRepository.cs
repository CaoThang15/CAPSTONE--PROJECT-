using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<string> GetUniqueSlug(long id, string name);
    }
}
