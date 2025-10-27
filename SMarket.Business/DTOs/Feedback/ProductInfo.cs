namespace SMarket.Business.DTOs.Feedback
{
    public class ProductInfo
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Slug { get; set; }
    }
}