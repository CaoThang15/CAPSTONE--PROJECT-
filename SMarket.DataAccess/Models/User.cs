using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            SellProducts = new HashSet<Product>();
            CartItems = new HashSet<CartItem>();
            UserVouchers = new HashSet<UserVoucher>();
            Orders = new HashSet<Order>();
            PersonalNotifications = new HashSet<PersonalNotification>();
        }

        [Required, MaxLength(255)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public required string Password { get; set; }

        public string? Avatar { get; set; }

        public string? Address { get; set; }

        public string? Ward { get; set; }

        public string? Province { get; set; }

        public int RoleId { get; set; }

        public Role? Role { get; set; }

        public ICollection<Product> SellProducts { get; set; }

        public ICollection<CartItem> CartItems { get; set; }

        public ICollection<UserVoucher> UserVouchers { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<PersonalNotification> PersonalNotifications { get; set; }
    }
}
