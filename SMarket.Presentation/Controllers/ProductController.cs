using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMarket.Business.DTOs.Product;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility;
using SMarket.Utility.Enums;
using System.Security.Claims;

namespace SMarket.Presentation.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Truyền vào Order: desc, asc.
        /// Truyền vào OrderBy: Created_At, Name, Price.
        /// Truyền vào Page &gt;= 1.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> SearchProducts([FromQuery] ListProductSearchCondition searchCondition)
        {
            try
            {
                var products = await _productService.GetListProductsAsync(searchCondition);

                return Ok(new Response
                {
                    Message = "Products retrieved successfully.",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve products.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("get/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Product not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Product retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve product.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> GetProductBySlug(string slug)
        {
            try
            {
                var product = await _productService.GetProductBySlugAsync(slug);
                if (product == null)
                {
                    return NotFound(new Response
                    {
                        Message = "Product not found."
                    });
                }

                return Ok(new Response
                {
                    Message = "Product retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to retrieve product.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> CreateProduct(CreateOrUpdateProductDto createProductDto)
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
                        Message = "Invalid product data.",
                        Data = errors
                    });
                }

                await _productService.CreateProductAsync(createProductDto);

                return Ok(new Response
                {
                    Message = "Product created successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to create product.",
                    Data = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> UpdateProduct(int id, CreateOrUpdateProductDto updateProductDto)
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
                        Message = "Invalid product data.",
                        Data = errors
                    });
                }

                await _productService.UpdateProductAsync(id, updateProductDto);

                return Ok(new Response
                {
                    Message = "Product updated successfully.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    Message = "Failed to update product.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);

                return Ok(new Response
                {
                    Message = "Product deleted successfully."
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
                    Message = "Failed to delete product.",
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// Nếu là create sản phẩm: truyền id = 0.
        /// Nếu là update sản phẩm: truyền id = id sản phẩm.
        /// </summary>
        [HttpGet("gen-slug")]
        [AllowAnonymous]
        // [Authorize(Roles = nameof(RoleType.Admin))]
        public async Task<ActionResult<Response>> GetUniqueProductSlug(int id, string name)
        {
            try
            {
                var slug = await _productService.GetUniqueProductSlug(id, name);

                return Ok(new Response
                {
                    Message = "Get slug successfully.",
                    Data = slug
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
                    Message = "Failed to get slug.",
                    Data = ex.Message
                });
            }
        }
    }
}
