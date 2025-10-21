namespace SMarket.Business.DTOs.Cart
{
    public class CartSummaryDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(item => item.Quantity);
        public double TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
