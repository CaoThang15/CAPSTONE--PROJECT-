using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Voucher;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using SMarket.Utility.Enums;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    [Authorize]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly INotificationService _notificationService;

        public VoucherController(IVoucherService voucherService, INotificationService notificationService)
        {
            _voucherService = voucherService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> GetAllVouchers()
        {
            try
            {
                var vouchers = await _voucherService.GetAllVouchersAsync();

                return Ok(new Response
                {
                    Message = "Vouchers retrieved successfully.",
                    Data = vouchers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve vouchers.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetVoucherById(int id)
        {
            try
            {
                var voucher = await _voucherService.GetVoucherByIdAsync(id);

                if (voucher == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Voucher not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Voucher retrieved successfully.",
                    Data = voucher
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve voucher.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Response>> GetVoucherByCode(string code)
        {
            try
            {
                var voucher = await _voucherService.GetVoucherByCodeAsync(code);

                if (voucher == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Voucher not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Voucher retrieved successfully.",
                    Data = voucher
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve voucher.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> CreateVoucher(CreateVoucherDto createVoucherDto)
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
                        Message = "Invalid voucher data.",
                        Data = errors
                    });
                }

                var voucher = await _voucherService.CreateVoucherAsync(createVoucherDto);

                return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.Id }, new Response
                {
                    Message = "Voucher created successfully.",
                    Data = voucher
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to create voucher.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> UpdateVoucher(int id, UpdateVoucherDto updateVoucherDto)
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
                        Message = "Invalid voucher data.",
                        Data = errors
                    });
                }

                var voucher = await _voucherService.UpdateVoucherAsync(id, updateVoucherDto);

                return Ok(new Response
                {
                    Message = "Voucher updated successfully.",
                    Data = voucher
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update voucher.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> DeleteVoucher(int id)
        {
            try
            {
                await _voucherService.DeleteVoucherAsync(id);

                return Ok(new Response
                {
                    Message = "Voucher deleted successfully."
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
                    Message = "Failed to delete voucher.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<Response>> GetActiveVouchers()
        {
            try
            {
                var vouchers = await _voucherService.GetActiveVouchersAsync();

                return Ok(new Response
                {
                    Message = "Active vouchers retrieved successfully.",
                    Data = vouchers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve active vouchers.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("my-vouchers")]
        [Authorize()]
        public async Task<ActionResult<Response>> GetMyVouchers()
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

                var vouchers = await _voucherService.GetUserVouchersAsync(userId);

                return Ok(new Response
                {
                    Message = "User vouchers retrieved successfully.",
                    Data = vouchers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve user vouchers.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("assign")]
        public async Task<ActionResult<Response>> AssignVoucherToUser(AssignVoucherDto assignVoucherDto)
        {
            try
            {
                var voucher = await _voucherService.AssignVoucherToUserAsync(assignVoucherDto.UserId, assignVoucherDto.VoucherId);

                await _notificationService.NotifyVoucherAssigned(
                    assignVoucherDto.UserId,
                    assignVoucherDto.VoucherId,
                    voucher.Code
                );

                return Ok(new Response
                {
                    Message = "Voucher assigned to user successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to assign voucher to user.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("assign/{voucherId}")]
        public async Task<ActionResult<Response>> RemoveVoucherFromUser(AssignVoucherDto assignVoucherDto)
        {
            try
            {
                await _voucherService.RemoveVoucherFromUserAsync(assignVoucherDto.UserId, assignVoucherDto.VoucherId);

                return Ok(new Response
                {
                    Message = "Voucher removed from user successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to remove voucher from user.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<Response>> GetVoucherStatuses()
        {
            try
            {
                var statuses = await _voucherService.GetVoucherStatusesAsync();

                return Ok(new Response
                {
                    Message = "Voucher statuses retrieved successfully.",
                    Data = statuses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve voucher statuses.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult<Response>> ValidateVoucher(ApplyVoucherDto voucherDto)
        {
            try
            {
                var result = await _voucherService.ValidateVoucherAsync(voucherDto.VoucherId);

                return Ok(new Response
                {
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to check voucher availability.",
                    Data = ex.Message
                });
            }
        }
    }
}
