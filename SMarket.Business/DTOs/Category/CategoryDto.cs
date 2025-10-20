using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs
{
    public class CategoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
    }
}
