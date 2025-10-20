using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs;
using SMarket.Business.DTOs.Upload;
using SMarket.Business.Enums;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using SMarket.Utility.Enums;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadImageController : ControllerBase
    {
        private static readonly string[] PERMITTED_FILE_EXTENSIONS = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly long FILE_SIZE_LIMIT = 10 * 1024 * 1024; // 10MB
        private ICloudinaryService _cloudinaryService;
        public UploadImageController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No file uploaded."
                });
            }
            try
            {
                var fileExt = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExt) || !PERMITTED_FILE_EXTENSIONS.Contains(fileExt))
                {
                    return BadRequest(new Response
                    {
                        Data = null,
                        Message = "Invalid file type. Only .jpg, .jpeg, .png, .webp are allowed."
                    });
                }

                if (request.File.Length > FILE_SIZE_LIMIT)
                {
                    return BadRequest(new Response
                    {
                        Data = null,
                        Message = "File size exceeds the 10MB limit."
                    });
                }

                using var stream = request.File.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImage(stream, request.Folder);

                return Ok(new Response
                {
                    Data = new { ImageUrl = imageUrl },
                    Message = "Upload image successfully."
                });
            }
            catch
            {
                return StatusCode(500, new Response
                {
                    Data = null,
                    Message = "An error occurred while uploading the image.",
                });
            }
        }

        [HttpDelete("image")]
        public async Task<IActionResult> DeleteImage([FromQuery] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return BadRequest(new Response
                {
                    Data = null,
                    Message = "Image URL is required."
                });
            }

            try
            {
                var isDeleted = await _cloudinaryService.DeleteImage(imageUrl);
                if (!isDeleted)
                {
                    return NotFound(new Response
                    {
                        Data = null,
                        Message = "Image not found or already deleted."
                    });
                }

                return Ok(new Response
                {
                    Data = null,
                    Message = "Image deleted successfully."
                });
            }
            catch
            {
                return StatusCode(500, new Response
                {
                    Data = null,
                    Message = "An error occurred while deleting the image.",
                });
            }
        }
    }
}
