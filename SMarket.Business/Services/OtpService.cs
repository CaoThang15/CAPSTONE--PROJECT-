using System.Collections.Concurrent;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;

namespace SMarket.Business.Services
{
    public class InMemoryOtpService : IOtpService
    {
        private readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry, User user)> _store
            = new();

        public void SaveOtp(string key, string otp, DateTime expiry, User user)
        {
            _store[key] = (otp, expiry, user);
        }

        public (string Otp, DateTime Expiry, User user)? GetOtp(string key)
        {
            return _store.TryGetValue(key, out var value) ? value : null;
        }

        public void RemoveOtp(string key)
        {
            _store.TryRemove(key, out _);
        }
    }
}