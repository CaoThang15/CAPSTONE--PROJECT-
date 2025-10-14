using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
