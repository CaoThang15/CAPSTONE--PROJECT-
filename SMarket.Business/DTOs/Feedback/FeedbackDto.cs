using SMarket.Business.DTOs.Product;

namespace SMarket.Business.DTOs.Feedback
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public int Rate { get; set; }

        public ProductInfo? ProductInfo { get; set; }

        public UserDto? User { get; set; }

        public SharedFileDto SharedFile { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}