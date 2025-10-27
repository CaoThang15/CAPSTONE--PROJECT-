using System.Threading.Tasks;
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
                    return BadRequest(
                        new Response { Message = "Email is already registered." }
                    );
                }

                var cred = new CredentialDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password,
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

                _authService.SetTokenCookie(Response, token);

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

                _authService.SetTokenCookie(Response, token);

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
            var token = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new Response
                {
                    Message = "No token found in cookies."
                });
            }

            var expiry = _authService.GetTokenExpiry(token);

            _tokenBlacklistService.Blacklist(token, expiry);

            _authService.RemoveTokenCookie(Response);

            return Ok(new Response
            {
                Message = "Logged out successfully"
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<Response>> ForgotPassword(ForgotPasswordDto req)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(req.Email);

                if (user != null)
                {
                    _authService.SendOtpToEmail(new CredentialDto { Email = req.Email });
                }

                return Ok(new Response
                {
                    Message = "Please check your email to reset your password."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to process forgot password request.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("verify-forgot-password-otp")]
        public async Task<ActionResult<Response>> VerifyForgotPasswordOtp(VerifyOtpRequest req)
        {
            try
            {
                var cred = _authService.VerifyOtp(req.Email, req.Otp);

                if (cred == null)
                {
                    return Unauthorized(new { message = "Invalid or expired OTP." });
                }

                var resetToken = _authService.GeneratePasswordResetToken(req.Email);

                return Ok(new Response
                {
                    Message = "OTP verified. You can now reset your password.",
                    Data = new { ResetToken = resetToken }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to verify OTP.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<Response>> ResetPassword(ResetPasswordDto req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>());
                    return BadRequest(new Response
                    {
                        Message = "Invalid data.",
                        Data = errors
                    });
                }

                var email = _authService.ValidatePasswordResetToken(req.ResetToken);
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new Response
                    {
                        Message = "Invalid or expired reset token."
                    });
                }

                await _userService.ChangePasswordAsync(email, req.NewPassword);

                return Ok(new Response
                {
                    Message = "Password reset successful. You can now login with your new password."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to reset password.",
                    Data = ex.Message
                });
            }
        }
    }
}
