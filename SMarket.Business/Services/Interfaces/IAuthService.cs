using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface IAuthService
    {
        void SendOtpToEmail(CredentialDto cred);
        CredentialDto? VerifyOtp(string email, string otp);
        Task<bool> IsCredentialValidAsync(string email, string password);
    }
}
