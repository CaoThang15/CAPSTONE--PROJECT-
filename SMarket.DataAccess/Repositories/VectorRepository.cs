using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using Npgsql;
using System.Text.Json;
using Pgvector;

namespace SMarket.DataAccess.Repositories
{
    public class VectorRepository : IVectorRepository
    {
        private readonly AppDbContext _context;

        public VectorRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Thêm hoặc cập nhật vector của sản phẩm (embedding)
        /// </summary>
        public async Task UpsertProductVectorAsync(Product product, List<Property> properties, float[] vector)
        {
            var productVector = await _context.ProductVectors
                .FirstOrDefaultAsync(p => p.ProductId == product.Id);

            if (productVector == null)
            {
                productVector = new ProductVector
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description ?? "",
                    Price = product.Price,
                    Properties = JsonSerializer.Serialize(properties),
                    Embedding = new Vector(vector),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ProductVectors.Add(productVector);
            }
            else
            {
                productVector.Name = product.Name;
                productVector.Description = product.Description ?? "";
                productVector.Price = product.Price;
                productVector.Properties = JsonSerializer.Serialize(properties);
                productVector.Embedding = new Vector(vector);
                productVector.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Tìm danh sách sản phẩm gần nhất theo vector query
        /// </summary>
        public async Task<List<ProductVector>> SearchSimilarAsync(float[] queryVector, int k = 5)
        {
            // EF không parse được toán tử <-> nên dùng FromSqlRaw
            var sql = """
                SELECT *
                FROM "ProductVectors"
                ORDER BY "Embedding" <-> @q::vector
                LIMIT @k;
                """;

            var qParam = new NpgsqlParameter("q", queryVector);
            var kParam = new NpgsqlParameter("k", k);

            return await _context.ProductVectors
                .FromSqlRaw(sql, qParam, kParam)
                .ToListAsync();
        }
    }
}
