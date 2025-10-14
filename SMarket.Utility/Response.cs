using System.Text.Json.Serialization;

namespace SMarket.Utility
{
    public class Response
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }
}