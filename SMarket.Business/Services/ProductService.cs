using Microsoft.AspNetCore.Http.Features;
using SMarket.Business.DTOs;
using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Product;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomMapper _mapper;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, ICustomMapper mapper)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<ProductItemDto>> GetListProductsAsync(ListProductSearchCondition searchCondition)
        {
            var user = await _userRepository.GetAllAsync();
            var productsPaging = await _productRepository.GetListProductsAsync(searchCondition);
            var productDtos = _mapper.Map<Product, ProductItemDto>(productsPaging);
            var total = await _productRepository.GetCountProductsAsync(searchCondition);

            return new PaginationResult<ProductItemDto>(
                   currentPage: searchCondition.Page,
                   pageSize: searchCondition.PageSize,
                   totalItems: total,
                   items: productDtos
               );
        }

        public async Task<ProductItemDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id)
                ?? throw new NotFoundException("Product not found");
            return _mapper.Map<Product, ProductItemDto>(product);
        }

        public async Task<ProductItemDto> GetProductBySlugAsync(string slug)
        {
            var user = await _userRepository.GetAllAsync();

            var product = await _productRepository.GetProductBySlugAsync(slug)
                ?? throw new NotFoundException("Product not found");
            return _mapper.Map<Product, ProductItemDto>(product);
        }

        public async Task CreateProductAsync(CreateOrUpdateProductDto createProductDto)
        {
            var productSlug = await this.GetUniqueProductSlug(0, createProductDto.Name);
            var product = _mapper.Map<CreateOrUpdateProductDto, Product>(createProductDto);
            product.Slug = productSlug;
            var sharedFiles = _mapper.Map<CreateOrUpdateProductDto, List<SharedFile>>(createProductDto);
            var properties = _mapper.Map<CreateOrUpdateProductDto, List<Property>>(createProductDto);
            await _productRepository.CreateProductAsync(product, sharedFiles, properties);
        }

        public async Task UpdateProductAsync(int id, CreateOrUpdateProductDto updateProductDto)
        {
            updateProductDto.Id = id;
            var product = _mapper.Map<CreateOrUpdateProductDto, Product>(updateProductDto);
            var sharedFiles = _mapper.Map<CreateOrUpdateProductDto, List<SharedFile>>(updateProductDto);
            var properties = _mapper.Map<CreateOrUpdateProductDto, List<Property>>(updateProductDto);
            await _productRepository.UpdateProductAsync(product, sharedFiles, properties);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteProductAsync(id);
        }

        public async Task HideOrShowBySeller(int id, bool isHide)
        {
            await _productRepository.HideOrShowBySeller(id, isHide);
        }

        public async Task HideOrShowByAdmin(int id, bool isHide)
        {
            await _productRepository.HideOrShowByAdmin(id, isHide);
        }

        public async Task<string> GetUniqueProductSlug(long id, string name)
        {
            return await _productRepository.GetUniqueProductSlug(id, name);
        }
    }
}
