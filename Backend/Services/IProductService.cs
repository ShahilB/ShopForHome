using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Services
{
    public interface IProductService
    {
        // Product CRUD operations
        Task<ProductListResponseDto> GetProductsAsync(ProductFilterDto filter);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> GetProductBySkuAsync(string sku);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductStockAsync(int id, int newStock);
        
        // Category operations
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        
        // Product Images
        Task<ProductImageDto> AddProductImageAsync(int productId, CreateProductImageDto createImageDto);
        Task<bool> DeleteProductImageAsync(int imageId);
        Task<bool> SetPrimaryImageAsync(int productId, int imageId);
        
        // Search and filtering
        Task<ProductListResponseDto> SearchProductsAsync(string searchTerm, ProductFilterDto? filter = null);
        Task<List<ProductDto>> GetFeaturedProductsAsync(int count = 10);
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId, int count = 20);
        Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int count = 5);
        
        // Stock management
        Task<List<ProductDto>> GetLowStockProductsAsync();
        Task<List<StockNotificationDto>> GetStockNotificationsAsync(bool unreadOnly = true);
        Task<bool> MarkStockNotificationAsReadAsync(int notificationId);
        
        // Bulk operations
        Task<BulkUploadResultDto> BulkUploadProductsAsync(List<BulkProductUploadDto> products);
        Task<bool> BulkUpdateProductStatusAsync(List<int> productIds, bool isActive);
        Task<bool> BulkDeleteProductsAsync(List<int> productIds);
        
        // Analytics
        Task<List<ProductDto>> GetTopRatedProductsAsync(int count = 10);
        Task<List<ProductDto>> GetBestSellingProductsAsync(int count = 10);
        Task<Dictionary<string, int>> GetProductCountByCategoryAsync();
    }

    public class StockNotificationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BulkUploadResultDto
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<ProductDto> CreatedProducts { get; set; } = new List<ProductDto>();
    }
}