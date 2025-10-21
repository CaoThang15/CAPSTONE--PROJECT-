using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }
}
