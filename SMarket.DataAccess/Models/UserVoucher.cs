namespace SMarket.DataAccess.Models
{
    public class UserVoucher : BaseEntity
    {
        public int UserId { get; set; }

        public int VoucherId { get; set; }

        public Voucher? Voucher { get; set; }
    }
}
