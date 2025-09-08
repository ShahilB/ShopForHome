using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopForHome.API.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DiscountPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        public int Stock { get; set; } = 0;

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

        [Column(TypeName = "decimal(8,2)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;

        public int ReviewCount { get; set; } = 0;

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ShoppingCart> ShoppingCartItems { get; set; } = new List<ShoppingCart>();
        public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<StockNotification> StockNotifications { get; set; } = new List<StockNotification>();

        [NotMapped]
        public decimal EffectivePrice => DiscountPrice ?? Price;

        [NotMapped]
        public bool IsOnSale => DiscountPrice.HasValue && DiscountPrice < Price;

        [NotMapped]
        public decimal DiscountPercentage => IsOnSale ? Math.Round(((Price - DiscountPrice!.Value) / Price) * 100, 2) : 0;

        [NotMapped]
        public bool IsLowStock => Stock <= MinStockLevel;

        [NotMapped]
        public bool IsOutOfStock => Stock <= 0;
    }
}