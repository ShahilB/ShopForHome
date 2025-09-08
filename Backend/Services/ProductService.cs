using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopForHomeDbContext _context;

        public ProductService(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListResponseDto> GetProductsAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Apply filters
            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.MinRating.HasValue)
                query = query.Where(p => p.Rating >= filter.MinRating.Value);

            if (!string.IsNullOrEmpty(filter.Brand))
                query = query.Where(p => p.Brand!.Contains(filter.Brand));

            if (!string.IsNullOrEmpty(filter.Color))
                query = query.Where(p => p.Color!.Contains(filter.Color));

            if (!string.IsNullOrEmpty(filter.Material))
                query = query.Where(p => p.Material!.Contains(filter.Material));

            if (filter.IsOnSale.HasValue && filter.IsOnSale.Value)
                query = query.Where(p => p.DiscountPrice.HasValue && p.DiscountPrice < p.Price);

            if (filter.IsFeatured.HasValue)
                query = query.Where(p => p.IsFeatured == filter.IsFeatured.Value);

            if (filter.InStock.HasValue && filter.InStock.Value)
                query = query.Where(p => p.Stock > 0);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description!.ToLower().Contains(searchTerm) ||
                    p.Brand!.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "rating" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Rating)
                    : query.OrderBy(p => p.Rating),
                "created" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => MapToProductDto(p))
                .ToListAsync();

            return new ProductListResponseDto
            {
                Products = products,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.Page < totalPages,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product != null ? MapToProductDto(product) : null;
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            return product != null ? MapToProductDto(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                DiscountPrice = createProductDto.DiscountPrice,
                SKU = createProductDto.SKU,
                Stock = createProductDto.Stock,
                MinStockLevel = createProductDto.MinStockLevel,
                CategoryId = createProductDto.CategoryId,
                Brand = createProductDto.Brand,
                Color = createProductDto.Color,
                Material = createProductDto.Material,
                Dimensions = createProductDto.Dimensions,
                Weight = createProductDto.Weight,
                ImageUrl = createProductDto.ImageUrl,
                IsFeatured = createProductDto.IsFeatured,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Add product images if provided
            if (createProductDto.ProductImages.Any())
            {
                foreach (var imageDto in createProductDto.ProductImages)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = imageDto.ImageUrl,
                        AltText = imageDto.AltText,
                        IsPrimary = imageDto.IsPrimary,
                        DisplayOrder = imageDto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ProductImages.Add(productImage);
                }
                await _context.SaveChangesAsync();
            }

            return await GetProductByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to retrieve created product");
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.DiscountPrice = updateProductDto.DiscountPrice;
            product.Stock = updateProductDto.Stock;
            product.MinStockLevel = updateProductDto.MinStockLevel;
            product.CategoryId = updateProductDto.CategoryId;
            product.Brand = updateProductDto.Brand;
            product.Color = updateProductDto.Color;
            product.Material = updateProductDto.Material;
            product.Dimensions = updateProductDto.Dimensions;
            product.Weight = updateProductDto.Weight;
            product.ImageUrl = updateProductDto.ImageUrl;
            product.IsActive = updateProductDto.IsActive;
            product.IsFeatured = updateProductDto.IsFeatured;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetProductByIdAsync(id);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductStockAsync(int id, int newStock)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Stock = newStock;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ProductCount = c.Products.Count(p => p.IsActive)
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                ProductCount = category.Products.Count(p => p.IsActive)
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                ImageUrl = createCategoryDto.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(category.Id) ?? throw new InvalidOperationException("Failed to retrieve created category");
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.ImageUrl = updateCategoryDto.ImageUrl;
            category.IsActive = updateCategoryDto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetCategoryByIdAsync(id);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return false;

            // Check if category has products
            if (category.Products.Any())
            {
                throw new InvalidOperationException("Cannot delete category with existing products");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductImageDto> AddProductImageAsync(int productId, CreateProductImageDto createImageDto)
        {
            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = createImageDto.ImageUrl,
                AltText = createImageDto.AltText,
                IsPrimary = createImageDto.IsPrimary,
                DisplayOrder = createImageDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = productImage.Id,
                ProductId = productImage.ProductId,
                ImageUrl = productImage.ImageUrl,
                AltText = productImage.AltText,
                IsPrimary = productImage.IsPrimary,
                DisplayOrder = productImage.DisplayOrder,
                CreatedAt = productImage.CreatedAt
            };
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return false;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetPrimaryImageAsync(int productId, int imageId)
        {
            // First, set all images for this product as non-primary
            var productImages = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            foreach (var image in productImages)
            {
                image.IsPrimary = image.Id == imageId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductListResponseDto> SearchProductsAsync(string searchTerm, ProductFilterDto? filter = null)
        {
            filter ??= new ProductFilterDto();
            filter.SearchTerm = searchTerm;
            return await GetProductsAsync(filter);
        }

        public async Task<List<ProductDto>> GetFeaturedProductsAsync(int count = 10)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId, int count = 20)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.Name)
                .Take(count)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int count = 5)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return new List<ProductDto>();

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != productId && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Stock <= p.MinStockLevel && p.IsActive)
                .OrderBy(p => p.Stock)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<List<StockNotificationDto>> GetStockNotificationsAsync(bool unreadOnly = true)
        {
            var query = _context.StockNotifications
                .Include(sn => sn.Product)
                .AsQueryable();

            if (unreadOnly)
                query = query.Where(sn => !sn.IsRead);

            return await query
                .OrderByDescending(sn => sn.CreatedAt)
                .Select(sn => new StockNotificationDto
                {
                    Id = sn.Id,
                    ProductId = sn.ProductId,
                    ProductName = sn.Product.Name,
                    Message = sn.Message,
                    IsRead = sn.IsRead,
                    CreatedAt = sn.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> MarkStockNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.StockNotifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BulkUploadResultDto> BulkUploadProductsAsync(List<BulkProductUploadDto> products)
        {
            var result = new BulkUploadResultDto
            {
                TotalProcessed = products.Count
            };

            foreach (var productDto in products)
            {
                try
                {
                    // Find or create category
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == productDto.CategoryName);

                    if (category == null)
                    {
                        category = new Category
                        {
                            Name = productDto.CategoryName,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    // Check if SKU already exists
                    if (await _context.Products.AnyAsync(p => p.SKU == productDto.SKU))
                    {
                        result.Errors.Add($"Product with SKU '{productDto.SKU}' already exists");
                        result.ErrorCount++;
                        continue;
                    }

                    var product = new Product
                    {
                        Name = productDto.Name,
                        Description = productDto.Description,
                        Price = productDto.Price,
                        DiscountPrice = productDto.DiscountPrice,
                        SKU = productDto.SKU,
                        Stock = productDto.Stock,
                        MinStockLevel = productDto.MinStockLevel,
                        CategoryId = category.Id,
                        Brand = productDto.Brand,
                        Color = productDto.Color,
                        Material = productDto.Material,
                        Dimensions = productDto.Dimensions,
                        Weight = productDto.Weight,
                        ImageUrl = productDto.ImageUrl,
                        IsFeatured = productDto.IsFeatured,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    var createdProduct = await GetProductByIdAsync(product.Id);
                    if (createdProduct != null)
                    {
                        result.CreatedProducts.Add(createdProduct);
                    }

                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error creating product '{productDto.Name}': {ex.Message}");
                    result.ErrorCount++;
                }
            }

            return result;
        }

        public async Task<bool> BulkUpdateProductStatusAsync(List<int> productIds, bool isActive)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                product.IsActive = isActive;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkDeleteProductsAsync(List<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductDto>> GetTopRatedProductsAsync(int count = 10)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive && p.Rating > 0)
                .OrderByDescending(p => p.Rating)
                .ThenByDescending(p => p.ReviewCount)
                .Take(count)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetBestSellingProductsAsync(int count = 10)
        {
            // This would typically be based on order data, but for now we'll use review count as a proxy
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.ReviewCount)
                .ThenByDescending(p => p.Rating)
                .Take(count)
                .Select(p => MapToProductDto(p))
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetProductCountByCategoryAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new { c.Name, ProductCount = c.Products.Count(p => p.IsActive) })
                .ToDictionaryAsync(x => x.Name, x => x.ProductCount);
        }

        private static ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                SKU = product.SKU,
                Stock = product.Stock,
                MinStockLevel = product.MinStockLevel,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Brand = product.Brand,
                Color = product.Color,
                Material = product.Material,
                Dimensions = product.Dimensions,
                Weight = product.Weight,
                Rating = product.Rating,
                ReviewCount = product.ReviewCount,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                ProductImages = product.ProductImages?.Select(pi => new ProductImageDto
                {
                    Id = pi.Id,
                    ProductId = pi.ProductId,
                    ImageUrl = pi.ImageUrl,
                    AltText = pi.AltText,
                    IsPrimary = pi.IsPrimary,
                    DisplayOrder = pi.DisplayOrder,
                    CreatedAt = pi.CreatedAt
                }).ToList() ?? new List<ProductImageDto>()
            };
        }
    }
}