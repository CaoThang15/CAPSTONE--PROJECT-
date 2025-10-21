using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Voucher
{
    public class UpdateVoucherDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Discount type cannot exceed 50 characters")]
        public string? DiscountType { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Discount amount must be greater than 0")]
        public float? DiscountAmount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be at least 1")]
        public int? UsageLimit { get; set; }

        public int? StatusId { get; set; }
    }
}
