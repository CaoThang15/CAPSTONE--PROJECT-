using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Voucher
{
    public class ApplyVoucherDto
    {
        [Required(ErrorMessage = "Voucher code is required")]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }
    }

    public class VoucherApplicationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
        public VoucherDto? Voucher { get; set; }
    }
}
