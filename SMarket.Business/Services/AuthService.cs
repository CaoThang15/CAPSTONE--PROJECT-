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

        public void SendOtpToEmail(CredentialDto cred)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _otpService.SaveOtp(cred.Email, otp, DateTime.UtcNow.AddMinutes(5), cred);

            _taskQueue.QueueOtp(() =>
            {
                _emailService.SendEmailAsync(
                    cred.Email,
                    "Your OTP Code",
                    $"<h3>Your OTP is: <b>{otp}</b></h3><p>It will expire in 5 minutes.</p>"
                );
            });
        }

        public CredentialDto? VerifyOtp(string email, string otp)
        {
            var stored = _otpService.GetOtp(email);
            if (stored.HasValue && stored.Value.Expiry > DateTime.UtcNow && stored.Value.Otp == otp)
            {
                var cred = stored.Value.Cred;
                _otpService.RemoveOtp(email);
                return cred;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid or expired OTP");
            }
        }

        public async Task<bool> IsCredentialValidAsync(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    return false;
                }

                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (Exception)
            {
                return false;
            }
        }

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
