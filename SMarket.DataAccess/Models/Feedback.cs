using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Feedback : BaseEntity
    {
        [Required, MaxLength(255)]
        public string? Content { get; set; }

        public int Rate { get; set; }

        public int? FromUserId { get; set; }

        public User? FromUser { get; set; }

        public int? ToUserId { get; set; }

        public User? ToUser { get; set; }

        public int FileId { get; set; }

        public SharedFile? SharedFile { get; set; }
    }
}
