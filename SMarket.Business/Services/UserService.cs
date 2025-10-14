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

        Task<IEnumerable<UserDto>> IUserService.GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            throw new NotImplementedException();
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