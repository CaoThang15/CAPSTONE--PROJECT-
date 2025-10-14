using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs
{
    public class AuthResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("user")]
        public UserDto? UserDto { get; set; }
    }
}
