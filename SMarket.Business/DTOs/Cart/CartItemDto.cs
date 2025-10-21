namespace SMarket.Business.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice => UnitPrice * Quantity;
        public int StockQuantity { get; set; }
        public bool IsAvailable => StockQuantity >= Quantity;
    }
}
