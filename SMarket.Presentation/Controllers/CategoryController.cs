using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs;
using SMarket.Business.Enums;
using SMarket.Business.Services.Interfaces;
using SMarket.Utility;
using SMarket.Utility.Enums;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                return Ok(new Response
                {
                    Message = "Categories retrieved successfully.",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve categories.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Category not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Category retrieved successfully.",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve category.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid category data.",
                        Data = errors
                    });
                }

                var category = await _categoryService.CreateCategoryAsync(createCategoryDto);

                return Ok(new Response
                {
                    Message = "Category created successfully.",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to create category.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return BadRequest(new Response
                    {
                        Message = "Invalid category data.",
                        Data = errors
                    });
                }

                var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

                return Ok(new Response
                {
                    Message = "Category updated successfully.",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update category.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);

                return Ok(new Response
                {
                    Message = "Category deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new Response
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to delete category.",
                    Data = ex.Message
                });
            }
        }
    }
}
