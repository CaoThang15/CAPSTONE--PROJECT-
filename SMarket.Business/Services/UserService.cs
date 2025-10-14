using SMarket.Business.DTOs;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using AutoMapper;

namespace SMarket.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> CreateBuyerAsync(CredentialDto cred)
        {
            var createUser = new User
            {
                Email = cred.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(cred.Password),
                Name = $"User_{Guid.NewGuid().ToString().Substring(0, 8)}",
                RoleId = 2
            };

            var user = await _userRepository.AddAsync(createUser);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateSellerAsync(CreateUserDto createUserDto)
        {
            var createUser = new User
            {
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                Name = $"User_{Guid.NewGuid().ToString().Substring(0, 8)}",
                RoleId = 3
            };

            var user = await _userRepository.AddAsync(createUser);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAdminAsync(CreateUserDto createUserDto)
        {
            var createUser = new User
            {
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                Name = $"User_{Guid.NewGuid().ToString().Substring(0, 8)}",
                RoleId = 1
            };

            var user = await _userRepository.AddAsync(createUser);
            return _mapper.Map<UserDto>(user);
        }

        public Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}