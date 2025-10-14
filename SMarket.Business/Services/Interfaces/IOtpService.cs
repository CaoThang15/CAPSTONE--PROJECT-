using SMarket.DataAccess.Models;

namespace SMarket.Business.Services.Interfaces
{
    public interface IOtpService
    {
        void SaveOtp(string key, string otp, DateTime expiry, User user);
        (string Otp, DateTime Expiry, User user)? GetOtp(string key);
        void RemoveOtp(string key);
    }
}