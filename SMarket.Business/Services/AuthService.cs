using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMarket.Business.DTOs;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Repositories.Interfaces;
using AutoMapper;

namespace SMarket.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = GenerateJwtToken(user.Id, user.Email, user.Role.Name);

            return new AuthResponseDto
            {
                AccessToken = token,
                UserDto = _mapper.Map<UserDto>(user)
            };
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }

        // public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        // {
        // if (await UserExistsAsync(registerDto.Email))
        // {
        //     throw new InvalidOperationException("User with this email already exists");
        // }

        // // Get default role (assuming RoleId 1 is User role)
        // var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == 1);
        // if (defaultRole == null)
        // {
        //     throw new InvalidOperationException("Default role not found");
        // }

        // var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // var user = new User
        // {
        //     Name = registerDto.Name,
        //     Email = registerDto.Email,
        //     Phone = registerDto.Phone,
        //     Password = hashedPassword,
        //     Address = registerDto.Address,
        //     RoleId = defaultRole.Id,
        //     CreatedAt = DateTime.UtcNow,
        //     UpdatedAt = DateTime.UtcNow
        // };

        // _context.Users.Add(user);
        // await _context.SaveChangesAsync();

        // await _context.Entry(user).Reference(u => u.Role).LoadAsync();

        // var token = GenerateJwtToken(user);
        // var expiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!));

        // return new AuthResponseDto
        // {
        //     Token = token,
        //     Email = user.Email!,
        //     Name = user.Name!,
        //     Role = user.Role?.Name ?? "User",
        //     ExpiresAt = expiryTime
        // };
        // }

        private string GenerateJwtToken(int userId, string email, string role)
        {
            var jwtSection = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
