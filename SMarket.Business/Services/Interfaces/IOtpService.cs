using SMarket.Business.DTOs;
using SMarket.DataAccess.Models;

namespace SMarket.Business.Services.Interfaces
{
    public interface IOtpService
    {
        void SaveOtp(string key, string otp, DateTime expiry, CredentialDto? cred);
        (string Otp, DateTime Expiry, CredentialDto? Cred)? GetOtp(string key);
        void RemoveOtp(string key);
    }
}