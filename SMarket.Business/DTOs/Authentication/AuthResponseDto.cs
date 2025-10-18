namespace SMarket.Business.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public UserDto? UserDto { get; set; }
    }
}
