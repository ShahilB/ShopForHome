using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public CartController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get user's cart items
        /// </summary>
        /// <returns>List of cart items</returns>
        [HttpGet]
        public async Task<ActionResult<List<CartItemDto>>> GetCartItems()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var cartItems = await _context.ShoppingCart
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Include(c => c.Product.ProductImages)
                .Where(c => c.UserId == userId.Value)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Product = MapToProductDto(c.Product),
                    TotalPrice = c.Product.DiscountPrice.HasValue ? 
                        c.Product.DiscountPrice.Value * c.Quantity : 
                        c.Product.Price * c.Quantity
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <param name="request">Add to cart request</param>
        /// <returns>Created cart item</returns>
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if product exists and is active
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            // Check if item already exists in cart
            var existingItem = await _context.ShoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId.Value && c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new CartItemDto
                {
                    Id = existingItem.Id,
                    UserId = existingItem.UserId,
                    ProductId = existingItem.ProductId,
                    Quantity = existingItem.Quantity,
                    CreatedAt = existingItem.CreatedAt,
                    UpdatedAt = existingItem.UpdatedAt,
                    Product = MapToProductDto(product),
                    TotalPrice = product.DiscountPrice.HasValue ? 
                        product.DiscountPrice.Value * existingItem.Quantity : 
                        product.Price * existingItem.Quantity
                });
            }

            // Create new cart item
            var cartItem = new ShoppingCart
            {
                UserId = userId.Value,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ShoppingCart.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartItems), new CartItemDto
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                CreatedAt = cartItem.CreatedAt,
                UpdatedAt = cartItem.UpdatedAt,
                Product = MapToProductDto(product),
                TotalPrice = product.DiscountPrice.HasValue ? 
                    product.DiscountPrice.Value * cartItem.Quantity : 
                    product.Price * cartItem.Quantity
            });
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        /// <param name="id">Cart item ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated cart item</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(int id, [FromBody] UpdateCartItemDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cartItem = await _context.ShoppingCart
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Include(c => c.Product.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId.Value);

            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            cartItem.Quantity = request.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new CartItemDto
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                CreatedAt = cartItem.CreatedAt,
                UpdatedAt = cartItem.UpdatedAt,
                Product = MapToProductDto(cartItem.Product),
                TotalPrice = cartItem.Product.DiscountPrice.HasValue ? 
                    cartItem.Product.DiscountPrice.Value * cartItem.Quantity : 
                    cartItem.Product.Price * cartItem.Quantity
            });
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        /// <param name="id">Cart item ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var cartItem = await _context.ShoppingCart
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId.Value);

            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            _context.ShoppingCart.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item removed from cart" });
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        /// <returns>Success status</returns>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var cartItems = await _context.ShoppingCart
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            _context.ShoppingCart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cart cleared successfully" });
        }

        /// <summary>
        /// Get cart summary
        /// </summary>
        /// <returns>Cart summary with totals</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<CartSummaryDto>> GetCartSummary()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var cartItems = await _context.ShoppingCart
                .Include(c => c.Product)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            var subtotal = cartItems.Sum(c => 
                c.Product.DiscountPrice.HasValue ? 
                    c.Product.DiscountPrice.Value * c.Quantity : 
                    c.Product.Price * c.Quantity);

            var tax = subtotal * 0.08m; // 8% tax
            var shipping = subtotal > 50 ? 0 : 10; // Free shipping over $50
            var total = subtotal + tax + shipping;

            return Ok(new CartSummaryDto
            {
                TotalItems = cartItems.Sum(c => c.Quantity),
                Subtotal = subtotal,
                Tax = tax,
                Shipping = shipping,
                Total = total
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
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

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProductDto Product { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }

    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
    }
}