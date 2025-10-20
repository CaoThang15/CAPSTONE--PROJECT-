using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(255)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
    }
}
