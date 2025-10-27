using SMarket.Business.DTOs.Product;

namespace SMarket.Business.DTOs.Feedback
{
    public class CreateOrUpdateFeedbackDto
    {
        public int? Id { get; set; }

        public string? Content { get; set; }

        public int Rate { get; set; }

        public int ProductId { get; set; }

        public int? UserId { get; set; }

        public SharedFileDto? SharedFile { get; set; }
    }
}