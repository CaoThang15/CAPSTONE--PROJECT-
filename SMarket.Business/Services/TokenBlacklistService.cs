// This file is kept for reference but not used since we're using InMemoryTokenBlacklistService
// You can delete this file if you prefer

/*
using Microsoft.EntityFrameworkCore;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;

namespace SMarket.Business.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly AppDbContext _context;

        public TokenBlacklistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task BlacklistTokenAsync(string tokenId, int userId, DateTime expiryDate)
        {
            var blacklistedToken = new BlacklistedToken
            {
                TokenId = tokenId,
                UserId = userId,
                ExpiryDate = expiryDate,
                BlacklistedAt = DateTime.UtcNow
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string tokenId)
        {
            return await _context.BlacklistedTokens
                .AnyAsync(bt => bt.TokenId == tokenId && bt.ExpiryDate > DateTime.UtcNow);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.BlacklistedTokens
                .Where(bt => bt.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.BlacklistedTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
*/
