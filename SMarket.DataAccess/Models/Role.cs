using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Role : BaseEntity
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        [Required, MaxLength(255)]
        public string? Name { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; }
    }
}
