using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class CategoryProperty : BaseEntity
    {
        [Required, MaxLength(255)]
        public string? Name { get; set; }

        public int? CategoryId { get; set; }

        // Navigation
        public Category? Category { get; set; }
    }
}
