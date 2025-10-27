namespace SMarket.Business.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? ShippingAddress { get; set; }
        public int WardId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int? SellerId { get; set; }
        public string? SellerName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? VoucherId { get; set; }
        public float? DiscountAmount { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = [];
    }
}