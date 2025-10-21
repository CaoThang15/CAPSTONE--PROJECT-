using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class ProductProperty : BaseEntity
    {
        [Required, MaxLength(255)]
        public string? Value { get; set; }

        public int? ProductId { get; set; }

        // Navigation
        public Product? Product { get; set; }

        public int? PropertyId { get; set; }

        public CategoryProperty? Property { get; set; }
    }
}
