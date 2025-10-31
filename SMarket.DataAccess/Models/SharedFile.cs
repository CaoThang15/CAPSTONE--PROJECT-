using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class SharedFile : BaseEntity
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        public string? Path { get; set; }

        public Product? Product { get; set; }

        public int? ProductId { get; set; }
        
        public Category? Category { get; set; }
    }
}
