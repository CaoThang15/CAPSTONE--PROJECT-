using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}
