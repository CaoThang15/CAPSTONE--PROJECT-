using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Product;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginationResult<ProductItemDto>> GetListProductsAsync(ListProductSearchCondition searchCondition);
        Task<ProductItemDto> GetProductByIdAsync(int id);
        Task<ProductItemDto> GetProductBySlugAsync(string slug);
        Task CreateProductAsync(CreateOrUpdateProductDto createProductDto);
        Task UpdateProductAsync(int cateId, CreateOrUpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);
        Task<string> GetUniqueProductSlug(long id, string name);
    }
}
