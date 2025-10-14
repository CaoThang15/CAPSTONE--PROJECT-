using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs
{
    public class CreateUserDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("roleId")]
        public int RoleId { get; set; } = 1; // Default to User role
    }
}
