using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopForHome.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCoupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCoupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "User"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    FinalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BillingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCoupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCoupons_DiscountCoupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "DiscountCoupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockNotifications_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlist_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlist_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2224), "Home furniture including sofas, chairs, tables, and storage solutions", "/assets/images/categories/furniture.jpg", true, "Furniture", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2516) },
                    { 2, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2768), "Decorative items to beautify your home including wall art, vases, and ornaments", "/assets/images/categories/home-decor.jpg", true, "Home Décor", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2769) },
                    { 3, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2774), "Indoor and outdoor lighting solutions including lamps, chandeliers, and LED lights", "/assets/images/categories/lighting.jpg", true, "Lighting", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2775) },
                    { 4, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2779), "Kitchen appliances, cookware, and dining accessories", "/assets/images/categories/kitchen-dining.jpg", true, "Kitchen & Dining", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2780) },
                    { 5, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2783), "Bedroom furniture and accessories including beds, mattresses, and bedding", "/assets/images/categories/bedroom.jpg", true, "Bedroom", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2784) },
                    { 6, new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2788), "Bathroom fixtures, accessories, and storage solutions", "/assets/images/categories/bathroom.jpg", true, "Bathroom", new DateTime(2025, 9, 7, 18, 15, 20, 319, DateTimeKind.Utc).AddTicks(2788) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "PhoneNumber", "Role", "State", "UpdatedAt", "ZipCode" },
                values: new object[] { 1, "123 Admin Street", "Admin City", "USA", new DateTime(2025, 9, 7, 18, 15, 20, 320, DateTimeKind.Utc).AddTicks(9535), "admin@shopforhome.com", "Admin", true, "User", "AQAAAAEAACcQAAAAEJ5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==", "+1234567890", "Admin", "Admin State", new DateTime(2025, 9, 7, 18, 15, 20, 320, DateTimeKind.Utc).AddTicks(9744), null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 1, "ComfortLiving", 1, "Gray", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7293), "Comfortable 3-piece sectional sofa with premium fabric upholstery", "120x80x35 inches", 999.99m, "/assets/images/products/sofa-sectional.jpg", true, true, "Fabric", 10, "Modern Sectional Sofa", 1299.99m, 4.5m, 23, "SOFA-001", 15, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7515), 85.5m },
                    { 2, "WoodCraft", 1, "Natural Oak", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7670), "Solid oak dining table seats 6 people comfortably", "72x36x30 inches", null, "/assets/images/products/dining-table.jpg", true, true, "Solid Oak", 10, "Oak Dining Table", 899.99m, 4.8m, 15, "TABLE-001", 8, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7671), 65.0m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 3, "ErgoMax", 1, "Black", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7700), "High-back ergonomic office chair with lumbar support", "26x26x45 inches", 279.99m, "/assets/images/products/office-chair.jpg", true, "Mesh", 10, "Ergonomic Office Chair", 349.99m, 4.3m, 41, "CHAIR-001", 25, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7701), 22.0m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 4, "ArtisticVisions", 2, "Multicolor", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7709), "Modern abstract canvas painting perfect for living room", "24x36 inches", null, "/assets/images/products/wall-art.jpg", true, true, "Canvas", 10, "Abstract Canvas Wall Art", 129.99m, 4.2m, 18, "ART-001", 30, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7709), 2.5m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 5, "CeramicCraft", 2, "White", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7716), "Handcrafted ceramic vase with elegant design", "8x8x12 inches", 59.99m, "/assets/images/products/ceramic-vase.jpg", true, "Ceramic", 10, "Ceramic Decorative Vase", 79.99m, 4.6m, 12, "VASE-001", 20, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7717), 3.2m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 6, "CozyHome", 2, "Blue", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7723), "Set of 4 decorative throw pillows with removable covers", "18x18 inches", null, "/assets/images/products/throw-pillows.jpg", true, true, "Cotton", 10, "Throw Pillow Set", 49.99m, 4.4m, 35, "PILLOW-001", 50, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7723), 1.8m },
                    { 7, "LuxuryLights", 3, "Crystal", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7730), "Elegant crystal chandelier with LED bulbs", "24x24x30 inches", 449.99m, "/assets/images/products/chandelier.jpg", true, true, "Crystal & Metal", 10, "Crystal Chandelier", 599.99m, 4.9m, 8, "LIGHT-001", 5, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7731), 15.5m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 8, "ModernLights", 3, "Black", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7755), "Minimalist table lamp with adjustable brightness", "8x8x18 inches", null, "/assets/images/products/table-lamp.jpg", true, "Metal", 10, "Modern Table Lamp", 89.99m, 4.1m, 27, "LAMP-001", 35, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7756), 4.2m },
                    { 9, "SmartHome", 3, "White", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7763), "Smart LED floor lamp with remote control", "12x12x60 inches", 159.99m, "/assets/images/products/floor-lamp.jpg", true, "Metal & Plastic", 10, "LED Floor Lamp", 199.99m, 4.7m, 22, "LAMP-002", 18, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7764), 8.5m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 10, "ChefMaster", 4, "Stainless Steel", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7770), "Professional 12-piece stainless steel cookware set", "Various sizes", 229.99m, "/assets/images/products/cookware-set.jpg", true, true, "Stainless Steel", 10, "Stainless Steel Cookware Set", 299.99m, 4.6m, 31, "COOK-001", 12, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7771), 18.0m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 11, "BambooKitchen", 4, "Natural", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7776), "Large bamboo cutting board with juice groove", "18x12x1 inches", null, "/assets/images/products/cutting-board.jpg", true, "Bamboo", 10, "Wooden Cutting Board", 39.99m, 4.3m, 19, "BOARD-001", 45, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7777), 2.8m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 12, "SleepWell", 5, "White", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7784), "Queen size memory foam mattress with cooling gel", "60x80x12 inches", 599.99m, "/assets/images/products/mattress.jpg", true, true, "Memory Foam", 10, "Memory Foam Mattress", 799.99m, 4.7m, 45, "MATT-001", 10, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7784), 55.0m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 13, "ModernBedroom", 5, "Walnut", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7886), "Modern nightstand with 2 drawers and USB charging", "18x16x24 inches", null, "/assets/images/products/nightstand.jpg", true, "Wood", 10, "Bedside Nightstand", 149.99m, 4.4m, 16, "NIGHT-001", 22, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7887), 25.0m },
                    { 14, "AquaLux", 6, "Chrome", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7894), "Large rainfall shower head with multiple spray settings", "8x8x4 inches", 119.99m, "/assets/images/products/shower-head.jpg", true, "Stainless Steel", 10, "Rainfall Shower Head", 159.99m, 4.5m, 33, "SHOWER-001", 28, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7895), 3.5m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "Color", "CreatedAt", "Description", "Dimensions", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Material", "MinStockLevel", "Name", "Price", "Rating", "ReviewCount", "SKU", "Stock", "UpdatedAt", "Weight" },
                values: new object[] { 15, "BambooLux", 6, "Beige", new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7901), "Luxury bamboo bath towel set - ultra soft and absorbent", "Various sizes", null, "/assets/images/products/towel-set.jpg", true, true, "Bamboo Fiber", 10, "Bamboo Bath Towel Set", 89.99m, 4.8m, 28, "TOWEL-001", 40, new DateTime(2025, 9, 7, 18, 15, 20, 321, DateTimeKind.Utc).AddTicks(7901), 2.2m });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCoupons_Code",
                table: "DiscountCoupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCoupons_ValidFrom_ValidTo",
                table: "DiscountCoupons",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId_UserId",
                table: "ProductReviews",
                columns: new[] { "ProductId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsFeatured",
                table: "Products",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Rating",
                table: "Products",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ProductId",
                table: "ShoppingCart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_UserId_ProductId",
                table: "ShoppingCart",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockNotifications_ProductId",
                table: "StockNotifications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_CouponId",
                table: "UserCoupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_UserId",
                table: "UserCoupons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_UserId_CouponId",
                table: "UserCoupons",
                columns: new[] { "UserId", "CouponId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_ProductId",
                table: "Wishlist",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_UserId",
                table: "Wishlist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_UserId_ProductId",
                table: "Wishlist",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "StockNotifications");

            migrationBuilder.DropTable(
                name: "UserCoupons");

            migrationBuilder.DropTable(
                name: "Wishlist");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "DiscountCoupons");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
