using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IVectorRepository
    {
        Task UpsertProductVectorAsync(Product product, List<Property> properties, float[] vector);
        Task<List<ProductVector>> SearchSimilarAsync(float[] queryVector, int k = 5);
    }
}
