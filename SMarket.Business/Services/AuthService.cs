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
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IBackgroundTaskQueue _taskQueue;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpService otpService, IBackgroundTaskQueue taskQueue)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
            _taskQueue = taskQueue;
        }

        public async Task LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            _otpService.SaveOtp(user.Email, otp, DateTime.UtcNow.AddMinutes(5), user);

            _taskQueue.QueueOtp(() =>
            {
                _emailService.SendEmailAsync(
        user.Email,
        "Your OTP Code",
        $"<h3>Your OTP is: <b>{otp}</b></h3><p>It will expire in 5 minutes.</p>"
        );
            });
        }

        public AuthResponseDto VerifyOtp(string email, string otp)
        {
            var stored = _otpService.GetOtp(email);
            if (stored.HasValue && stored.Value.Expiry > DateTime.UtcNow && stored.Value.Otp == otp)
            {
                _otpService.RemoveOtp(email);
                var token = GenerateJwtToken(stored.Value.user.Id, stored.Value.user.Email, stored.Value.user.Role.Name);

                return new AuthResponseDto
                {
                    AccessToken = token,
                    UserDto = _mapper.Map<UserDto>(stored.Value.user)
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid or expired OTP");
            }
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
