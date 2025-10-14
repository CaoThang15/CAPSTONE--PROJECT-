using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs
{
    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        public string Otp { get; set; } = string.Empty;
    }
}
