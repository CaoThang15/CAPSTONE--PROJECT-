using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Slug { get; set; }
    }
}
