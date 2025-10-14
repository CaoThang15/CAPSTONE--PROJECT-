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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                // var result = await _authService.RegisterAsync(registerDto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginRequestDto);
                return Ok(new Response
                {
                    Message = "Đăng nhập thành công",
                    Data = result
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }

        [HttpGet("public")]
        public IActionResult Public() =>
    Ok(new { message = "Anyone can see this" });

        [HttpGet("private")]
        [Authorize]
        public IActionResult Private() =>
            Ok(new { message = $"Hello {User.Identity?.Name}, you are authenticated!" });

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly() =>
            Ok(new { message = "Only Admins can access this" });
    }
}
