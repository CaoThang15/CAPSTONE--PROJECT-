using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class SystemNotification : BaseEntity
    {
        public SystemNotification()
        {
            PersonalNotifications = new HashSet<PersonalNotification>();
        }

        [Required, MaxLength(50)]
        public int Type { get; set; } // General / Voucher

        [Required]
        public string? Content { get; set; }

        public int? IdRefer { get; set; }

        public DateTime? TimeToSend { get; set; }

        public bool IsSent { get; set; }

        public int CreateByUser { get; set; }

        public ICollection<PersonalNotification> PersonalNotifications { get; set; }
    }
}
