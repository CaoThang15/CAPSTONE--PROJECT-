using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        public int WardId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int? VoucherId { get; set; }

        public User? User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public OrderStatus? Status { get; set; }
        public Voucher? Voucher { get; set; }
    }
}
