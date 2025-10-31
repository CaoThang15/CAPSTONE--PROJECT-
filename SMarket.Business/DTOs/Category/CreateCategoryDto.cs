using System.ComponentModel.DataAnnotations;
using SMarket.Business.DTOs.Product;

namespace SMarket.Business.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public SharedFileDto? Thumbnail { get; set; }
    }
}
