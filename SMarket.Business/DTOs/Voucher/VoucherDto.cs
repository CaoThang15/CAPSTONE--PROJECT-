namespace SMarket.Business.DTOs.Voucher
{
    public class VoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public float DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive => StatusId == 1 && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate && UsageCount < UsageLimit;
    }
}
