using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Models;

namespace ShopForHome.API.Data
{
    public class ShopForHomeDbContext : DbContext
    {
        public ShopForHomeDbContext(DbContextOptions<ShopForHomeDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<StockNotification> StockNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints
            
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("User");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.Price);
                entity.HasIndex(e => e.Rating);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsFeatured);
                
                entity.Property(e => e.Stock).HasDefaultValue(0);
                entity.Property(e => e.MinStockLevel).HasDefaultValue(10);
                entity.Property(e => e.Rating).HasDefaultValue(0);
                entity.Property(e => e.ReviewCount).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProductImage entity configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.IsPrimary).HasDefaultValue(false);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ShoppingCart entity configuration
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                
                entity.Property(e => e.Quantity).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Wishlist entity configuration
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.OrderDate);
                
                entity.Property(e => e.DiscountAmount).HasDefaultValue(0);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.PaymentStatus).HasDefaultValue("Pending");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem entity configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DiscountCoupon entity configuration
            modelBuilder.Entity<DiscountCoupon>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.ValidFrom, e.ValidTo });
                
                entity.Property(e => e.MinOrderAmount).HasDefaultValue(0);
                entity.Property(e => e.UsedCount).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // UserCoupon entity configuration
            modelBuilder.Entity<UserCoupon>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.CouponId }).IsUnique();
                
                entity.Property(e => e.IsUsed).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCoupons)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DiscountCoupon)
                    .WithMany(p => p.UserCoupons)
                    .HasForeignKey(d => d.CouponId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductReview entity configuration
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => new { e.ProductId, e.UserId }).IsUnique();
                
                entity.Property(e => e.IsApproved).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StockNotification entity configuration
            modelBuilder.Entity<StockNotification>(entity =>
            {
                entity.Property(e => e.IsRead).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.StockNotifications)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Furniture", Description = "Home furniture including sofas, chairs, tables, and storage solutions", ImageUrl = "/assets/images/categories/furniture.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 2, Name = "Home Décor", Description = "Decorative items to beautify your home including wall art, vases, and ornaments", ImageUrl = "/assets/images/categories/home-decor.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 3, Name = "Lighting", Description = "Indoor and outdoor lighting solutions including lamps, chandeliers, and LED lights", ImageUrl = "/assets/images/categories/lighting.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 4, Name = "Kitchen & Dining", Description = "Kitchen appliances, cookware, and dining accessories", ImageUrl = "/assets/images/categories/kitchen-dining.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 5, Name = "Bedroom", Description = "Bedroom furniture and accessories including beds, mattresses, and bedding", ImageUrl = "/assets/images/categories/bedroom.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 6, Name = "Bathroom", Description = "Bathroom fixtures, accessories, and storage solutions", ImageUrl = "/assets/images/categories/bathroom.jpg", CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Seed Admin User (password will be hashed in the service layer)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@shopforhome.com",
                    PasswordHash = "AQAAAAEAACcQAAAAEJ5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==", // This will be replaced with proper hash
                    Role = "Admin",
                    PhoneNumber = "+1234567890",
                    Address = "123 Admin Street",
                    City = "Admin City",
                    State = "Admin State",
                    Country = "USA",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // Seed Sample Products
            modelBuilder.Entity<Product>().HasData(
                // Furniture Category (ID: 1)
                new Product { Id = 1, Name = "Modern Sectional Sofa", Description = "Comfortable 3-piece sectional sofa with premium fabric upholstery", Price = 1299.99m, DiscountPrice = 999.99m, SKU = "SOFA-001", Stock = 15, CategoryId = 1, Brand = "ComfortLiving", Color = "Gray", Material = "Fabric", Dimensions = "120x80x35 inches", Weight = 85.5m, Rating = 4.5m, ReviewCount = 23, ImageUrl = "/assets/images/products/sofa-sectional.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 2, Name = "Oak Dining Table", Description = "Solid oak dining table seats 6 people comfortably", Price = 899.99m, SKU = "TABLE-001", Stock = 8, CategoryId = 1, Brand = "WoodCraft", Color = "Natural Oak", Material = "Solid Oak", Dimensions = "72x36x30 inches", Weight = 65.0m, Rating = 4.8m, ReviewCount = 15, ImageUrl = "/assets/images/products/dining-table.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 3, Name = "Ergonomic Office Chair", Description = "High-back ergonomic office chair with lumbar support", Price = 349.99m, DiscountPrice = 279.99m, SKU = "CHAIR-001", Stock = 25, CategoryId = 1, Brand = "ErgoMax", Color = "Black", Material = "Mesh", Dimensions = "26x26x45 inches", Weight = 22.0m, Rating = 4.3m, ReviewCount = 41, ImageUrl = "/assets/images/products/office-chair.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },

                // Home Décor Category (ID: 2)
                new Product { Id = 4, Name = "Abstract Canvas Wall Art", Description = "Modern abstract canvas painting perfect for living room", Price = 129.99m, SKU = "ART-001", Stock = 30, CategoryId = 2, Brand = "ArtisticVisions", Color = "Multicolor", Material = "Canvas", Dimensions = "24x36 inches", Weight = 2.5m, Rating = 4.2m, ReviewCount = 18, ImageUrl = "/assets/images/products/wall-art.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 5, Name = "Ceramic Decorative Vase", Description = "Handcrafted ceramic vase with elegant design", Price = 79.99m, DiscountPrice = 59.99m, SKU = "VASE-001", Stock = 20, CategoryId = 2, Brand = "CeramicCraft", Color = "White", Material = "Ceramic", Dimensions = "8x8x12 inches", Weight = 3.2m, Rating = 4.6m, ReviewCount = 12, ImageUrl = "/assets/images/products/ceramic-vase.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 6, Name = "Throw Pillow Set", Description = "Set of 4 decorative throw pillows with removable covers", Price = 49.99m, SKU = "PILLOW-001", Stock = 50, CategoryId = 2, Brand = "CozyHome", Color = "Blue", Material = "Cotton", Dimensions = "18x18 inches", Weight = 1.8m, Rating = 4.4m, ReviewCount = 35, ImageUrl = "/assets/images/products/throw-pillows.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },

                // Lighting Category (ID: 3)
                new Product { Id = 7, Name = "Crystal Chandelier", Description = "Elegant crystal chandelier with LED bulbs", Price = 599.99m, DiscountPrice = 449.99m, SKU = "LIGHT-001", Stock = 5, CategoryId = 3, Brand = "LuxuryLights", Color = "Crystal", Material = "Crystal & Metal", Dimensions = "24x24x30 inches", Weight = 15.5m, Rating = 4.9m, ReviewCount = 8, ImageUrl = "/assets/images/products/chandelier.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 8, Name = "Modern Table Lamp", Description = "Minimalist table lamp with adjustable brightness", Price = 89.99m, SKU = "LAMP-001", Stock = 35, CategoryId = 3, Brand = "ModernLights", Color = "Black", Material = "Metal", Dimensions = "8x8x18 inches", Weight = 4.2m, Rating = 4.1m, ReviewCount = 27, ImageUrl = "/assets/images/products/table-lamp.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 9, Name = "LED Floor Lamp", Description = "Smart LED floor lamp with remote control", Price = 199.99m, DiscountPrice = 159.99m, SKU = "LAMP-002", Stock = 18, CategoryId = 3, Brand = "SmartHome", Color = "White", Material = "Metal & Plastic", Dimensions = "12x12x60 inches", Weight = 8.5m, Rating = 4.7m, ReviewCount = 22, ImageUrl = "/assets/images/products/floor-lamp.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },

                // Kitchen & Dining Category (ID: 4)
                new Product { Id = 10, Name = "Stainless Steel Cookware Set", Description = "Professional 12-piece stainless steel cookware set", Price = 299.99m, DiscountPrice = 229.99m, SKU = "COOK-001", Stock = 12, CategoryId = 4, Brand = "ChefMaster", Color = "Stainless Steel", Material = "Stainless Steel", Dimensions = "Various sizes", Weight = 18.0m, Rating = 4.6m, ReviewCount = 31, ImageUrl = "/assets/images/products/cookware-set.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 11, Name = "Wooden Cutting Board", Description = "Large bamboo cutting board with juice groove", Price = 39.99m, SKU = "BOARD-001", Stock = 45, CategoryId = 4, Brand = "BambooKitchen", Color = "Natural", Material = "Bamboo", Dimensions = "18x12x1 inches", Weight = 2.8m, Rating = 4.3m, ReviewCount = 19, ImageUrl = "/assets/images/products/cutting-board.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },

                // Bedroom Category (ID: 5)
                new Product { Id = 12, Name = "Memory Foam Mattress", Description = "Queen size memory foam mattress with cooling gel", Price = 799.99m, DiscountPrice = 599.99m, SKU = "MATT-001", Stock = 10, CategoryId = 5, Brand = "SleepWell", Color = "White", Material = "Memory Foam", Dimensions = "60x80x12 inches", Weight = 55.0m, Rating = 4.7m, ReviewCount = 45, ImageUrl = "/assets/images/products/mattress.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 13, Name = "Bedside Nightstand", Description = "Modern nightstand with 2 drawers and USB charging", Price = 149.99m, SKU = "NIGHT-001", Stock = 22, CategoryId = 5, Brand = "ModernBedroom", Color = "Walnut", Material = "Wood", Dimensions = "18x16x24 inches", Weight = 25.0m, Rating = 4.4m, ReviewCount = 16, ImageUrl = "/assets/images/products/nightstand.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },

                // Bathroom Category (ID: 6)
                new Product { Id = 14, Name = "Rainfall Shower Head", Description = "Large rainfall shower head with multiple spray settings", Price = 159.99m, DiscountPrice = 119.99m, SKU = "SHOWER-001", Stock = 28, CategoryId = 6, Brand = "AquaLux", Color = "Chrome", Material = "Stainless Steel", Dimensions = "8x8x4 inches", Weight = 3.5m, Rating = 4.5m, ReviewCount = 33, ImageUrl = "/assets/images/products/shower-head.jpg", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 15, Name = "Bamboo Bath Towel Set", Description = "Luxury bamboo bath towel set - ultra soft and absorbent", Price = 89.99m, SKU = "TOWEL-001", Stock = 40, CategoryId = 6, Brand = "BambooLux", Color = "Beige", Material = "Bamboo Fiber", Dimensions = "Various sizes", Weight = 2.2m, Rating = 4.8m, ReviewCount = 28, ImageUrl = "/assets/images/products/towel-set.jpg", IsFeatured = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }
    }
}