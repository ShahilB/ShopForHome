using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.API.DTOs;
using ShopForHome.API.Services;

namespace ShopForHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;

        public CategoriesController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _productService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            return Ok(category);
        }

        /// <summary>
        /// Create a new category (Admin only)
        /// </summary>
        /// <param name="createCategoryDto">Category creation data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = await _productService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="updateCategoryDto">Category update data</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _productService.UpdateCategoryAsync(id, updateCategoryDto);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            return Ok(category);
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _productService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(new { message = "Category deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get products in a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of products in category</returns>
        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductDto>>> GetCategoryProducts(int id, [FromQuery] int count = 20)
        {
            var products = await _productService.GetProductsByCategoryAsync(id, count);
            return Ok(products);
        }
    }
}