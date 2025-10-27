using SMarket.Business.DTOs;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICustomMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, ICustomMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<Category, CategoryDto>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? _mapper.Map<Category, CategoryDto>(category) : null;
        }

        public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            var category = await _categoryRepository.GetBySlugAsync(slug);
            return category != null ? _mapper.Map<Category, CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrWhiteSpace(createCategoryDto.Slug))
            {
                createCategoryDto.Slug = Helpers.GenerateSlug(createCategoryDto.Name);
            }

            if (await _categoryRepository.SlugExistsAsync(createCategoryDto.Slug))
            {
                throw new InvalidOperationException($"Category with slug '{createCategoryDto.Slug}' already exists.");
            }

            var category = _mapper.Map<CreateCategoryDto, Category>(createCategoryDto);
            var createdCategory = await _categoryRepository.AddAsync(category);
            return _mapper.Map<Category, CategoryDto>(createdCategory);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int cateId, UpdateCategoryDto updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(cateId);
            if (existingCategory == null)
            {
                throw new ArgumentException($"Category with ID {cateId} not found.");
            }

            if (string.IsNullOrWhiteSpace(updateCategoryDto.Slug))
            {
                updateCategoryDto.Slug = Helpers.GenerateSlug(updateCategoryDto.Name);
            }

            if (await _categoryRepository.SlugExistsAsync(updateCategoryDto.Slug, cateId))
            {
                throw new InvalidOperationException($"Category with slug '{updateCategoryDto.Slug}' already exists.");
            }

            _mapper.Map(updateCategoryDto, existingCategory);
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            return _mapper.Map<Category, CategoryDto>(updatedCategory);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (!await _categoryRepository.ExistsAsync(id))
            {
                throw new ArgumentException($"Category with ID {id} not found.");
            }

            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<string> GetUniqueSlug(long id, string name)
        {
            return await _categoryRepository.GetUniqueSlug(id, name);
        }
    }
}
