using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Product
{
    public class CreateOrUpdateProductDto
    {
        public int? Id { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public int StockQuantity { get; set; }

        public string? Note { get; set; }

        public bool IsNew { get; set; }

        public bool IsAdminHide { get; set; }

        public bool IsHide { get; set; }

        public int SellerId { get; set; }

        public List<SharedFileDto> SharedFiles { get; set; } = [];

        public List<ProductPropertyDto> Properties { get; set; } = [];
    }
}