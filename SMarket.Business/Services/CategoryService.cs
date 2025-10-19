using AutoMapper;
using SMarket.Business.DTOs;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrWhiteSpace(createCategoryDto.Slug))
            {
                createCategoryDto.Slug = GenerateSlug(createCategoryDto.Name);
            }

            if (await _categoryRepository.SlugExistsAsync(createCategoryDto.Slug))
            {
                throw new InvalidOperationException($"Category with slug '{createCategoryDto.Slug}' already exists.");
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            var createdCategory = await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryDto>(createdCategory);
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
                updateCategoryDto.Slug = GenerateSlug(updateCategoryDto.Name);
            }

            if (await _categoryRepository.SlugExistsAsync(updateCategoryDto.Slug, cateId))
            {
                throw new InvalidOperationException($"Category with slug '{updateCategoryDto.Slug}' already exists.");
            }

            _mapper.Map(updateCategoryDto, existingCategory);
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (!await _categoryRepository.ExistsAsync(id))
            {
                throw new ArgumentException($"Category with ID {id} not found.");
            }

            await _categoryRepository.DeleteAsync(id);
        }

        private string GenerateSlug(string name)
        {
            return name.ToLowerInvariant()
                      .Replace(" ", "-")
                      .Replace("&", "and")
                      .Replace("'", "")
                      .Replace("\"", "")
                      .Replace(",", "")
                      .Replace(".", "")
                      .Replace("!", "")
                      .Replace("?", "");
        }
    }
}
