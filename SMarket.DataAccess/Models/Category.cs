using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Required, MaxLength(255)]
        public string? Name { get; set; }
        [MaxLength(255)]
        public string? Slug { get; set; }

        public ICollection<Product> Products { get; set; }
        
        public int? ThumbnailId { get; set; }
        public SharedFile? Thumbnail { get; set; }
    }
}
