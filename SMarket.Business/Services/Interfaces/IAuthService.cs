using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}
