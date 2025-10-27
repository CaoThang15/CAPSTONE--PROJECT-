using Microsoft.AspNetCore.Http;
using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface IAuthService
    {
        void SendOtpToEmail(CredentialDto cred);
        CredentialDto? VerifyOtp(string email, string otp);
        Task<bool> IsCredentialValidAsync(string email, string password);
        string GenerateJwtToken(int userId, string email, int role);
        DateTime GetTokenExpiry(string token);
        void SetTokenCookie(HttpResponse response, string token);
        void RemoveTokenCookie(HttpResponse response);
        string GeneratePasswordResetToken(string email);
        string? ValidatePasswordResetToken(string token);
    }
}
