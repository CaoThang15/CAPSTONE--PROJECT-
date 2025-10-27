using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.DTOs.Common;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetListProductsAsync(ListProductSearchCondition searchCondition)
        {
            var query = _context.Products
                .Where(d => !d.IsDeleted)
                .Where(d => searchCondition.CategoryId == 0 || searchCondition.CategoryId == d.CategoryId)
                .Where(d => string.IsNullOrEmpty(searchCondition.KeyWord) || d.Name.Contains(searchCondition.KeyWord))
                .Where(d => searchCondition.MinPrice == null || searchCondition.MinPrice <= d.Price)
                .Where(d => searchCondition.MaxPrice == null || searchCondition.MaxPrice >= d.Price)
                .Where(d => searchCondition.IsNew == null || searchCondition.IsNew == d.IsNew)
                .Where(d => searchCondition.IsHide == null || searchCondition.IsHide == d.IsHide)
                .Where(d => searchCondition.IsAdminHide == null || searchCondition.IsAdminHide == d.IsAdminHide)
                .Where(d => searchCondition.SellerId == 0 || searchCondition.SellerId == d.SellerId)
                .Include(d => d.SharedFiles.Where(f => !f.IsDeleted))
                .Include(d => d.Seller)
                .Include(d => d.ProductProperties.Where(p => !p.IsDeleted && p.Property != null && !p.Property.IsDeleted))
                .ThenInclude(p => p!.Property)
                .AsQueryable();

            query = searchCondition.OrderBy?.ToLower() switch
            {
                "name" => searchCondition.Order.ToLower() == "asc"
                    ? query.OrderBy(d => d.Name)
                    : query.OrderByDescending(d => d.Name),

                "price" => searchCondition.Order.ToLower() == "asc"
                    ? query.OrderBy(d => d.Price)
                    : query.OrderByDescending(d => d.Price),

                "created_at" or _ => searchCondition.Order.ToLower() == "asc"
                    ? query.OrderBy(d => d.CreatedAt)
                    : query.OrderByDescending(d => d.CreatedAt)
            };

            // 🔽 Phân trang
            query = query
                .Skip((searchCondition.Page - 1) * searchCondition.PageSize)
                .Take(searchCondition.PageSize);

            return await query.ToListAsync();
        }

        public async Task<int> GetCountProductsAsync(ListProductSearchCondition searchCondition)
        {
            return await _context.Products
                .Where(d => !d.IsDeleted)
                .Where(d => searchCondition.CategoryId == 0 || searchCondition.CategoryId == d.CategoryId)
                .Where(d => string.IsNullOrEmpty(searchCondition.KeyWord) || searchCondition.KeyWord == d.Name)
                .Where(d => searchCondition.MinPrice == null || searchCondition.MinPrice <= d.Price)
                .Where(d => searchCondition.MaxPrice == null || searchCondition.MaxPrice >= d.Price)
                .Where(d => searchCondition.IsNew == null || searchCondition.IsNew == d.IsNew)
                .Where(d => searchCondition.IsHide == null || searchCondition.IsHide == d.IsHide)
                .Where(d => searchCondition.IsAdminHide == null || searchCondition.IsAdminHide == d.IsAdminHide)
                .Where(d => searchCondition.SellerId == 0 || searchCondition.SellerId == d.SellerId)
                .CountAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Where(d => !d.IsDeleted && d.Id == id)
                .Include(d => d.SharedFiles)
                .Include(d => d.Seller)
                .Include(d => d.ProductProperties)
                .ThenInclude(p => p!.Property)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Product?> GetProductBySlugAsync(string slug)
        {
            return await _context.Products
                .Where(d => !d.IsDeleted && d.Slug == slug)
                .Include(d => d.Seller)
                .Include(d => d.SharedFiles)
                .Include(d => d.ProductProperties)
                .ThenInclude(p => p!.Property)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task CreateProductAsync(Product product, List<SharedFile> sharedFiles, List<ProductProperty> properties)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.Products.Where(d => d.Slug == product.Slug && d.Id != product.Id).AnyAsync())
                    throw new InvalidOperationException("Slug already exists for another product.");

                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                foreach (var file in sharedFiles)
                {
                    file.ProductId = product.Id;
                    file.CreatedAt = DateTime.UtcNow;
                    await _context.SharedFiles.AddAsync(file);
                }

                foreach (var property in properties)
                {
                    property.ProductId = product.Id;
                    property.CreatedAt = DateTime.UtcNow;
                    await _context.ProductProperties.AddAsync(property);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateProductAsync(Product product, List<SharedFile> sharedFiles, List<ProductProperty> properties)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.Products.Where(d => d.Slug == product.Slug && d.Id != product.Id).AnyAsync())
                    throw new InvalidOperationException("Slug already exists for another product.");

                product.UpdatedAt = DateTime.UtcNow;
                _context.Products.Update(product);

                var oldSharedFile = await _context.SharedFiles.Where(d => d.ProductId == product.Id).ToListAsync();
                _context.SharedFiles.RemoveRange(oldSharedFile);
                foreach (var file in sharedFiles)
                {
                    file.ProductId = product.Id;
                    file.CreatedAt = DateTime.UtcNow;
                    await _context.SharedFiles.AddAsync(file);
                }

                var oldProperties = await _context.ProductProperties.Where(d => d.ProductId == product.Id).ToListAsync();
                _context.ProductProperties.RemoveRange(oldProperties);
                foreach (var property in properties)
                {
                    property.ProductId = product.Id;
                    property.CreatedAt = DateTime.UtcNow;
                    await _context.ProductProperties.AddAsync(property);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetUniqueProductSlug(long id, string name)
        {
            string baseSlug = Helpers.GenerateSlug(name);
            string uniqueSlug = baseSlug;
            var existingSlugs = await _context.Products
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
