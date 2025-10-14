using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task LoginAsync(LoginRequestDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        AuthResponseDto VerifyOtp(string email, string otp);
    }
}
