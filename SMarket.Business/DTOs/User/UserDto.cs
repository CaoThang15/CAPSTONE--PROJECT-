namespace SMarket.Business.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public string? Address { get; set; }

        public string? Province { get; set; }

        public string? Ward { get; set; }

        public int RoleId { get; set; }
    }

    public class UserProfileDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public string? Address { get; set; }

        public string? Ward { get; set; }

        public string? Province { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
