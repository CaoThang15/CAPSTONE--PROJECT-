using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMarket.Business.DTOs;
using SMarket.Business.Mappers;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.Utility.Enums;

namespace SMarket.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ICustomMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, ICustomMapper mapper, IEmailService emailService, IOtpService otpService, IBackgroundTaskQueue taskQueue, ITokenBlacklistService tokenBlacklistService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
            _taskQueue = taskQueue;
            _tokenBlacklistService = tokenBlacklistService;
            _httpContextAccessor = httpContextAccessor;
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

        public string GenerateJwtToken(int userId, string email, int roleId)
        {
            var jwtSection = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            string roleName = roleId switch
            {
                1 => nameof(RoleType.Admin),
                2 => nameof(RoleType.Buyer),
                3 => nameof(RoleType.Seller),
                _ => "Unknown"
            };

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, roleName),
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

        public DateTime GetTokenExpiry(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (exp == null) throw new Exception("Token has no exp claim");

            var expUnix = long.Parse(exp);
            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            return expDateTime;
        }

        public void SetTokenCookie(HttpResponse response, string token)
        {
            var expiry = GetTokenExpiry(token);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = expiry,
                Path = "/"
            };

            response.Cookies.Append("access_token", token, cookieOptions);
        }

        public void RemoveTokenCookie(HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            };

            response.Cookies.Append("access_token", "", cookieOptions);
        }

        public string GeneratePasswordResetToken(string email)
        {
            var jwtSection = _configuration.GetSection("JwtSettings");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT key is not configured"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("email", email),
                    new Claim("type", "password_reset")
                ]),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string? ValidatePasswordResetToken(string token)
        {
            try
            {
                var jwtSection = _configuration.GetSection("JwtSettings");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT key is not configured"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var tokenType = principal.FindFirst("type")?.Value;
                if (tokenType != "password_reset")
                    return null;

                return principal.FindFirst(ClaimTypes.Email)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
