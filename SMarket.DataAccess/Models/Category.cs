using System.ComponentModel.DataAnnotations;

namespace SMarket.DataAccess.Models
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Products = new HashSet<Product>();
            CategoryProperties = new HashSet<CategoryProperty>();
        }

        [Required, MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(255)]
        public string? Slug { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<CategoryProperty> CategoryProperties { get; set; }
    }
}
