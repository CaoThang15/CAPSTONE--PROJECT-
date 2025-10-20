using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SMarket.Utility.Enums;

namespace SMarket.Business.DTOs.Upload
{
    public class UploadImageRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        public string Folder { get; set; } = CloudinaryPathName.MAIN_FOLDER.ToString();
    }
}