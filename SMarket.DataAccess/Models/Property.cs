using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Property : BaseEntity
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Value { get; set; }

        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; }
    }
}
