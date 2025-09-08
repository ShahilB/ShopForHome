using System.ComponentModel.DataAnnotations;

namespace ShopForHome.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int MinStockLevel { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? Dimensions { get; set; }
        public decimal? Weight { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
        
        // Computed properties
        public decimal EffectivePrice => DiscountPrice ?? Price;
        public bool IsOnSale => DiscountPrice.HasValue && DiscountPrice < Price;
        public decimal DiscountPercentage => IsOnSale ? Math.Round(((Price - DiscountPrice!.Value) / Price) * 100, 2) : 0;
        public bool IsLowStock => Stock <= MinStockLevel;
        public bool IsOutOfStock => Stock <= 0;
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than 0")]
        public decimal? DiscountPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 10;

        [Required]
        public int CategoryId { get; set; }

        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public bool IsFeatured { get; set; } = false;

        public List<CreateProductImageDto> ProductImages { get; set; } = new List<CreateProductImageDto>();
    }

    public class UpdateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than 0")]
        public decimal? DiscountPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 10;

        [Required]
        public int CategoryId { get; set; }

        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductImageDto
    {
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }

    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public bool? IsOnSale { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? InStock { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "name"; // name, price, rating, created
        public string? SortOrder { get; set; } = "asc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ProductListResponseDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ProductCount { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }
    }

    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BulkProductUploadDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int MinStockLevel { get; set; } = 10;
        public string CategoryName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? Dimensions { get; set; }
        public decimal? Weight { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; } = false;
    }
}