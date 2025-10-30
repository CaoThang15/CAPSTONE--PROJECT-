using SMarket.Business.DTOs.Product;

namespace SMarket.Business.DTOs.AI
{
    public class ProductSuggestionInput
    {
        public string? CategoryName { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? Note { get; set; }

        public bool IsNew { get; set; }

        public List<PropertyDto> Properties { get; set; } = [];
    }
}
