namespace SMarket.Business.DTOs.Voucher
{
    public class AdminAssignVoucherDto
    {
        public List<int> UserIds { get; set; } = new List<int>();

        public List<int> VoucherIds { get; set; } = new List<int>();
    }
}