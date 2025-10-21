using System.ComponentModel.DataAnnotations;

namespace SMarket.Business.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int CartItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
