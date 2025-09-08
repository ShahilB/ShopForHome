using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.API.DTOs;
using ShopForHome.API.Services;

namespace ShopForHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        public async Task<ActionResult<ProductListResponseDto>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        /// <summary>
        /// Get product by SKU
        /// </summary>
        /// <param name="sku">Product SKU</param>
        /// <returns>Product details</returns>
        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<ProductDto>> GetProductBySku(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        /// <summary>
        /// Create a new product (Admin only)
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Delete a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(new { message = "Product deleted successfully" });
        }

        /// <summary>
        /// Update product stock (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="stock">New stock quantity</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductStock(int id, [FromBody] int stock)
        {
            var result = await _productService.UpdateProductStockAsync(id, stock);
            if (!result)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(new { message = "Stock updated successfully" });
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="filter">Additional filter criteria</param>
        /// <returns>Search results</returns>
        [HttpGet("search")]
        public async Task<ActionResult<ProductListResponseDto>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] ProductFilterDto? filter = null)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var result = await _productService.SearchProductsAsync(searchTerm, filter);
            return Ok(result);
        }

        /// <summary>
        /// Get featured products
        /// </summary>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of featured products</returns>
        [HttpGet("featured")]
        public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetFeaturedProductsAsync(count);
            return Ok(products);
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of products in category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(int categoryId, [FromQuery] int count = 20)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId, count);
            return Ok(products);
        }

        /// <summary>
        /// Get related products
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="count">Number of related products to return</param>
        /// <returns>List of related products</returns>
        [HttpGet("{id}/related")]
        public async Task<ActionResult<List<ProductDto>>> GetRelatedProducts(int id, [FromQuery] int count = 5)
        {
            var products = await _productService.GetRelatedProductsAsync(id, count);
            return Ok(products);
        }

        /// <summary>
        /// Get low stock products (Admin only)
        /// </summary>
        /// <returns>List of low stock products</returns>
        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ProductDto>>> GetLowStockProducts()
        {
            var products = await _productService.GetLowStockProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get top rated products
        /// </summary>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of top rated products</returns>
        [HttpGet("top-rated")]
        public async Task<ActionResult<List<ProductDto>>> GetTopRatedProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetTopRatedProductsAsync(count);
            return Ok(products);
        }

        /// <summary>
        /// Get best selling products
        /// </summary>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of best selling products</returns>
        [HttpGet("best-selling")]
        public async Task<ActionResult<List<ProductDto>>> GetBestSellingProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetBestSellingProductsAsync(count);
            return Ok(products);
        }

        /// <summary>
        /// Add product image (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="createImageDto">Image data</param>
        /// <returns>Created image</returns>
        [HttpPost("{id}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductImageDto>> AddProductImage(int id, [FromBody] CreateProductImageDto createImageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var image = await _productService.AddProductImageAsync(id, createImageDto);
            return Ok(image);
        }

        /// <summary>
        /// Delete product image (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="imageId">Image ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}/images/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductImage(int id, int imageId)
        {
            var result = await _productService.DeleteProductImageAsync(imageId);
            if (!result)
            {
                return NotFound(new { message = "Image not found" });
            }

            return Ok(new { message = "Image deleted successfully" });
        }

        /// <summary>
        /// Set primary product image (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="imageId">Image ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/images/{imageId}/primary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPrimaryImage(int id, int imageId)
        {
            var result = await _productService.SetPrimaryImageAsync(id, imageId);
            if (!result)
            {
                return NotFound(new { message = "Image not found" });
            }

            return Ok(new { message = "Primary image set successfully" });
        }

        /// <summary>
        /// Bulk update product status (Admin only)
        /// </summary>
        /// <param name="request">Bulk update request</param>
        /// <returns>Success status</returns>
        [HttpPatch("bulk/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkUpdateProductStatus([FromBody] BulkUpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.BulkUpdateProductStatusAsync(request.ProductIds, request.IsActive);
            return Ok(new { message = $"Updated {request.ProductIds.Count} products successfully" });
        }

        /// <summary>
        /// Bulk delete products (Admin only)
        /// </summary>
        /// <param name="request">Bulk delete request</param>
        /// <returns>Success status</returns>
        [HttpDelete("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteProducts([FromBody] BulkDeleteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.BulkDeleteProductsAsync(request.ProductIds);
            return Ok(new { message = $"Deleted {request.ProductIds.Count} products successfully" });
        }

        /// <summary>
        /// Get product count by category (Admin only)
        /// </summary>
        /// <returns>Product count by category</returns>
        [HttpGet("analytics/category-count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, int>>> GetProductCountByCategory()
        {
            var result = await _productService.GetProductCountByCategoryAsync();
            return Ok(result);
        }
    }

    public class BulkUpdateStatusRequest
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }
    }

    public class BulkDeleteRequest
    {
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}