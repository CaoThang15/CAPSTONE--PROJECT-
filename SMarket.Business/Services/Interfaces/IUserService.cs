using SMarket.Business.DTOs;

namespace SMarket.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CredentialDto cred);
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task ChangePasswordAsync(string email, string newPassword);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}