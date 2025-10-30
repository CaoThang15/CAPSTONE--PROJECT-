using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace SMarket.DataAccess.Models
{
    [Table("product_vectors")]
    public class ProductVector : BaseEntity
    {
        public long ProductId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Properties { get; set; }

        [Column("embedding", TypeName = "vector(768)")]
        public Vector Embedding { get; set; } = new Vector(new float[768]);
    }
}
