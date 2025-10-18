namespace SMarket.Business.DTOs
{
    public class CategoryPropertyDto
    {
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }
}
