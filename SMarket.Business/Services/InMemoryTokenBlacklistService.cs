using SMarket.Business.Services.Interfaces;
using System.Collections.Concurrent;

namespace SMarket.Business.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void Blacklist(string token, DateTime expiry)
        {
            _blacklistedTokens[token] = expiry;
        }

        public bool IsBlacklisted(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out var expiry))
            {
                if (expiry > DateTime.UtcNow) return true;
                _blacklistedTokens.TryRemove(token, out _);
            }
            return false;
        }

        public Task CleanupExpiredTokensAsync()
        {
            var currentTime = DateTime.UtcNow;
            var expiredTokens = _blacklistedTokens
                .Where(kvp => kvp.Value <= currentTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                _blacklistedTokens.TryRemove(token, out _);
            }

            return Task.CompletedTask;
        }
    }
}
