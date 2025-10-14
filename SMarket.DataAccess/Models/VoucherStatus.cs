using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class VoucherStatus : BaseEntity
    {
        public VoucherStatus()
        {
            Vouchers = new HashSet<Voucher>();
        }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        public ICollection<Voucher> Vouchers { get; set; }
    }
}
