using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Product : BaseEntity
    {
        public Product()
        {
            SharedFiles = new HashSet<SharedFile>();
            OrderDetails = new HashSet<OrderDetail>();
            Properties = new HashSet<Property>();
        }

        public int CategoryId { get; set; }

        [Required, MaxLength(255)]
        public string? Name { get; set; }

        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? Slug { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public string? Description { get; set; }

        public int StockQuantity { get; set; }

        public string? Note { get; set; }

        public bool IsNew { get; set; }

        public bool IsAdminHide { get; set; }

        public bool IsHide { get; set; }

        public int SellerId { get; set; }

        public Category? Category { get; set; }

        public User? Seller { get; set; }

        public ICollection<SharedFile> SharedFiles { get; set; }

        public ICollection<Property> Properties { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
