using System.ComponentModel.DataAnnotations;
using SMarket.Business.Enums;

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

        [Required]
        public RoleEnum Role { get; set; }
    }
}
