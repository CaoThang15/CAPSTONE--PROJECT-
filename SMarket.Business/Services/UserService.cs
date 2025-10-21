using SMarket.Business.DTOs;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMapper _mapper;

        public UserService(IUserRepository userRepository, ICustomMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<User, UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.Map<User, UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<User, UserDto>(users);
        }

        public async Task<UserDto> CreateUserAsync(CredentialDto cred)
        {
            var createUser = new User
            {
                Email = cred.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(cred.Password),
                Name = $"User_{Guid.NewGuid().ToString().Substring(0, 8)}",
                RoleId = (int)cred.Role
            };

            var user = await _userRepository.AddAsync(createUser);
            return _mapper.Map<User, UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            _mapper.Map(updateUserDto, existingUser);
            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<User, UserDto>(updatedUser);
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            if (BCrypt.Net.BCrypt.Verify(newPassword, user.Password))
            {
                throw new InvalidOperationException("New password must be different from current password.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }
    }
}