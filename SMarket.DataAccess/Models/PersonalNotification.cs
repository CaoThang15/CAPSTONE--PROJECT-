using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class PersonalNotification : BaseEntity
    {
        public int? SystemNotificationId { get; set; }

        [Required, MaxLength(50)]
        public int Type { get; set; } // General / Voucher / Order

        [Required]
        public string? Content { get; set; }

        public int ToUserId { get; set; }

        public User? ToUser { get; set; }

        public bool IsRead { get; set; }

        public int? IdRefer { get; set; }

        public DateTime? SendAt { get; set; }

        // Navigation
        public SystemNotification? SystemNotification { get; set; }
    }
}
