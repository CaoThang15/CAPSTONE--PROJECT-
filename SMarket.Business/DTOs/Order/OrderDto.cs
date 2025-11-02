namespace SMarket.Business.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? ShippingAddress { get; set; }
        public int WardId { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public UserDto? Customer { get; set; }
        public int? SellerId { get; set; }
        public UserDto? Seller { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? VoucherId { get; set; }
        public float? DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = [];
    }
}