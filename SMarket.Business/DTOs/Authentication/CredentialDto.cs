using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs
{
    public class CredentialDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
