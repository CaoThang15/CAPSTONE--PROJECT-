using SMarket.Business.DTOs.Product;

namespace SMarket.Business.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public SharedFileDto? Thumbnail { get; set; }
    }
}
