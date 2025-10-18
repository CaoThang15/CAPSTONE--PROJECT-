using SMarket.Business.Services.Interfaces;
using System.Collections.Concurrent;

namespace SMarket.Business.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly Dictionary<string, DateTime> _blacklistedTokens = new();

        public void Blacklist(string token, DateTime expiry)
        {
            _blacklistedTokens[token] = expiry;
        }

        public bool IsBlacklisted(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out var expiry))
            {
                if (expiry > DateTime.UtcNow) return true;
                _blacklistedTokens.Remove(token);
            }
            return false;
        }
    }
}
