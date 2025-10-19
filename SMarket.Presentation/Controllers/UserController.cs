using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<Response>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new Response
                    {
                        Message = "User not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "User profile retrieved successfully.",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve user profile.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("update")]
        public async Task<ActionResult<Response>> UpdateUser(UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Response
                    {
                        Message = "Invalid user data.",
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var currentUserId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                var user = await _userService.UpdateUserAsync(currentUserId, updateUserDto);

                return Ok(new Response
                {
                    Message = "User updated successfully.",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update user.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                return Ok(new Response
                {
                    Message = "Users retrieved successfully.",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve users.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);

                return Ok(new Response
                {
                    Message = "User deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to delete user.",
                    Data = ex.Message
                });
            }
        }
    }
}
