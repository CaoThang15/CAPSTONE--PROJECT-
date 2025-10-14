using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class OrderStatus : BaseEntity
    {
        [Required, MaxLength(100)]
        public string? Name { get; set; }
    }
}
