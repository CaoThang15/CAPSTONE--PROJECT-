using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetListFeedbacksAsync(ListFeedbackSearchCondition searchCondition)
        {
            return await _context.Feedbacks
                .Where(d => !d.IsDeleted)
                .Where(d => searchCondition.ProductId == 0 || searchCondition.ProductId == d.ProductId)
                .Where(d => searchCondition.UserId == 0 || searchCondition.UserId == d.UserId)
                .Include(d => d.SharedFile)
                .Include(d => d.User)
                .Include(d => d.Product)
                .Skip((searchCondition.Page - 1) * searchCondition.PageSize)
                .Take(searchCondition.PageSize).ToListAsync();
        }

        public async Task<int> GetCountFeedbacksAsync(ListFeedbackSearchCondition searchCondition)
        {
            return await _context.Feedbacks
                .Where(d => !d.IsDeleted)
                .Where(d => searchCondition.ProductId == 0 || searchCondition.ProductId == d.ProductId)
                .Where(d => searchCondition.UserId == 0 || searchCondition.UserId == d.UserId)
                .CountAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Where(d => !d.IsDeleted && d.Id == id)
                .Include(d => d.SharedFile)
                .Include(d => d.User)
                .Include(d => d.Product).FirstOrDefaultAsync();
        }

        public async Task CreateFeedbackAsync(Feedback feedback, SharedFile? sharedFile)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (sharedFile != null)
                {
                    sharedFile.CreatedAt = DateTime.UtcNow;
                    await _context.SharedFiles.AddAsync(sharedFile);
                    await _context.SaveChangesAsync();
                    feedback.SharedFileId = sharedFile.Id;
                }

                feedback.CreatedAt = DateTime.UtcNow;
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateFeedbackAsync(Feedback feedback, SharedFile? sharedFile)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (sharedFile != null)
                {
                    sharedFile.CreatedAt = DateTime.UtcNow;
                    await _context.SharedFiles.AddAsync(sharedFile);
                    await _context.SaveChangesAsync();
                    feedback.SharedFileId = sharedFile.Id;
                }

                feedback.UpdatedAt = DateTime.UtcNow;
                _context.Feedbacks.Update(feedback);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id)
                ?? throw new ArgumentException("Feedback not found");

            feedback.IsDeleted = true;
            feedback.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
