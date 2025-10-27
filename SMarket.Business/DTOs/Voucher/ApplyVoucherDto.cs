using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Voucher
{
    public class ApplyVoucherDto
    {
        public int VoucherId { get; set; }
    }

    public class VoucherApplicationResult
    {
        public string Message { get; set; } = string.Empty;
        public VoucherDto? Voucher { get; set; }
    }
}
