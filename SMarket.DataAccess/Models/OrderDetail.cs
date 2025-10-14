using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public Order? Order { get; set; }
    }
}
