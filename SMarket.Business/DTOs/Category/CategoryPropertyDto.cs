using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs
{
    public class CategoryPropertyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category_id")]
        public int? CategoryId { get; set; }
    }
}
