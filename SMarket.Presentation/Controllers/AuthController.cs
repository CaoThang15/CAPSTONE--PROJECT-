using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public AuthController(IAuthService authService, IUserService userService, ITokenBlacklistService tokenBlacklistService)
        {
            _authService = authService;
            _userService = userService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    Message = "Invalid registration data.",
                });
            }

            try
            {
                var user = await _userService.GetUserByEmailAsync(registerDto.Email);

                if (user != null)
                {
                    return BadRequest();
                }

                var cred = new CredentialDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    Role = registerDto.Role
                };

                _authService.SendOtpToEmail(cred);

                return Ok(new Response
                {
                    Message = "Please enter the OTP sent to your email."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(CredentialDto cred)
        {
            try
            {
                var isCredentialValid = await _authService.IsCredentialValidAsync(cred.Email, cred.Password);

                if (!isCredentialValid)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                _authService.SendOtpToEmail(cred);

                return Ok(new Response
                {
                    Message = "Please enter the OTP sent to your email.",
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
        }

        [HttpPost("verify-login-otp")]
        public async Task<ActionResult<Response>> VerifyLoginOtp(VerifyOtpRequest req)
        {
            try
            {
                _authService.VerifyOtp(req.Email, req.Otp);
                var user = await _userService.GetUserByEmailAsync(req.Email);

                if (user == null)
                {
                    return Unauthorized(new { message = "User not found." });
                }

                var token = _authService.GenerateJwtToken(user.Id, user.Email, user.RoleId);
                return Ok(new Response
                {
                    Message = "Login successful.",
                    Data = new AuthResponseDto { UserDto = user, AccessToken = token }
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Invalid or expired OTP." });
            }
        }

        [HttpPost("verify-register-otp")]
        public async Task<ActionResult<Response>> VerifyRegisterOtp(VerifyOtpRequest req)
        {
            try
            {
                var cred = _authService.VerifyOtp(req.Email, req.Otp);

                if (cred == null)
                {
                    return Unauthorized(new { message = "Invalid or expired OTP." });
                }

                var newUser = await _userService.CreateUserAsync(cred);
                var token = _authService.GenerateJwtToken(newUser.Id, newUser.Email, newUser.RoleId);

                return Ok(new Response
                {
                    Message = "Registration successful.",
                    Data = new AuthResponseDto { UserDto = newUser, AccessToken = token }
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Invalid or expired OTP." });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var expiry = _authService.GetTokenExpiry(token);

            _tokenBlacklistService.Blacklist(token, expiry);

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
