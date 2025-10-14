using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Voucher : BaseEntity
    {
        public Voucher()
        {
            UserVouchers = new HashSet<UserVoucher>();
            Orders = new HashSet<Order>();
        }

        [Required, MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? DiscountType { get; set; }

        public float DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }

        public int StatusId { get; set; }

        public VoucherStatus? Status { get; set; }
        public ICollection<UserVoucher> UserVouchers { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
