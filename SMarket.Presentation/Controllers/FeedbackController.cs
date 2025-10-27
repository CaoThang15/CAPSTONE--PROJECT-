using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Feedback;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility;
using SMarket.Utility.Enums;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/feedbacks")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService Feedbackservice)
        {
            _feedbackService = Feedbackservice;
        }

        [HttpGet("user")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetListUserFeedback([FromQuery] ListFeedbackSearchCondition searchCondition)
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
                searchCondition.UserId = userId;
                var feedbacks = await _feedbackService.GetListFeedbacksAsync(searchCondition);

                return Ok(new Response
                {
                    Message = "Feedbacks retrieved successfully.",
                    Data = feedbacks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve Feedbacks.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("product")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetListFeedbackFeedback([FromQuery] ListFeedbackSearchCondition searchCondition)
        {
            try
            {
                var feedbacks = await _feedbackService.GetListFeedbacksAsync(searchCondition);

                return Ok(new Response
                {
                    Message = "Feedbacks retrieved successfully.",
                    Data = feedbacks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve Feedbacks.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("get/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetFeedbackById(int id)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
                if (feedback == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Feedback not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Feedback retrieved successfully.",
                    Data = feedback
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve Feedback.",
                    Data = ex.Message
                });
            }
        }

        
        [HttpPost]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> CreateFeedback(CreateOrUpdateFeedbackDto createFeedbackDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid Feedback data.",
                        Data = errors
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                createFeedbackDto.UserId = userId;

                await _feedbackService.CreateFeedbackAsync(createFeedbackDto);

                return Ok(new Response
                {
                    Message = "Feedback created successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to create Feedback.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> UpdateFeedback(int id, CreateOrUpdateFeedbackDto updateFeedbackDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid Feedback data.",
                        Data = errors
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new Response
                    {
                        Message = "Invalid user token."
                    });
                }

                updateFeedbackDto.UserId = userId;

                await _feedbackService.UpdateFeedbackAsync(id, updateFeedbackDto);

                return Ok(new Response
                {
                    Message = "Feedback updated successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update Feedback.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> DeleteFeedback(int id)
        {
            try
            {
                await _feedbackService.DeleteFeedbackAsync(id);

                return Ok(new Response
                {
                    Message = "Feedback deleted successfully."
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
                    Message = "Failed to delete Feedback.",
                    Data = ex.Message
                });
            }
        }
    }
}
