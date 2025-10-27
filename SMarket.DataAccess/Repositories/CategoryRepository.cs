using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.CategoryProperties)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.CategoryProperties)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _context.Categories
                .Include(c => c.CategoryProperties)
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<string> GetUniqueSlug(long id, string name)
        {
            string baseSlug = Helpers.GenerateSlug(name);
            string uniqueSlug = baseSlug;
            var existingSlugs = await _context.Categories
                .Where(b => !string.IsNullOrEmpty(b.Slug) && b.Slug!.StartsWith(baseSlug) && (id == 0 || b.Id != id))
                .Select(b => b.Slug!)
                .ToListAsync();

            HashSet<string> slugSet = [.. existingSlugs];

            if (slugSet.Contains(uniqueSlug))
            {
                int count = 1;
                while (slugSet.Contains($"{baseSlug}-{count}"))
                {
                    count++;
                }
                uniqueSlug = $"{baseSlug}-{count}";
            }

            return uniqueSlug;
        }
    }
}
