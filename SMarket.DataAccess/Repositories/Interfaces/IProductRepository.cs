using SMarket.DataAccess.Models;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetListProductsAsync(ListProductSearchCondition searchCondition);
        Task<int> GetCountProductsAsync(ListProductSearchCondition searchCondition);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductBySlugAsync(string slug);
        Task CreateProductAsync(Product product, List<SharedFile> sharedFiles, List<ProductProperty> properties);
        Task UpdateProductAsync(Product product, List<SharedFile> sharedFiles, List<ProductProperty> properties);
        Task DeleteProductAsync(int id);
        Task<string> GetUniqueProductSlug(long id, string name);
    }
}
