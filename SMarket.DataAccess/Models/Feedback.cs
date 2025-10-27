using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Feedback : BaseEntity
    {
        [Required, MaxLength(255)]
        public string? Content { get; set; }

        public int Rate { get; set; }

        public int? UserId { get; set; }

        public User? User { get; set; }

        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        public int SharedFileId { get; set; }

        public SharedFile? SharedFile { get; set; }
    }
}
