using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Voucher
{
    public class CreateVoucherDto
    {
        [Required(ErrorMessage = "Voucher code is required")]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Discount type is required")]
        [StringLength(50, ErrorMessage = "Discount type cannot exceed 50 characters")]
        public string DiscountType { get; set; } = string.Empty; // "Percentage" or "Fixed"

        [Required(ErrorMessage = "Discount amount is required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Discount amount must be greater than 0")]
        public float DiscountAmount { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Usage limit is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be at least 1")]
        public int UsageLimit { get; set; }

        [Required(ErrorMessage = "Status ID is required")]
        public int StatusId { get; set; } = 1; // Default to Active
    }
}
