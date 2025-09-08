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
    public class WishlistController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public WishlistController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get user's wishlist items
        /// </summary>
        /// <returns>List of wishlist items</returns>
        [HttpGet]
        public async Task<ActionResult<List<WishlistItemDto>>> GetWishlistItems()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var wishlistItems = await _context.Wishlist
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Include(w => w.Product.ProductImages)
                .Where(w => w.UserId == userId.Value)
                .Select(w => new WishlistItemDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    ProductId = w.ProductId,
                    CreatedAt = w.CreatedAt,
                    Product = MapToProductDto(w.Product)
                })
                .ToListAsync();

            return Ok(wishlistItems);
        }

        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <param name="request">Add to wishlist request</param>
        /// <returns>Created wishlist item</returns>
        [HttpPost]
        public async Task<ActionResult<WishlistItemDto>> AddToWishlist([FromBody] AddToWishlistDto request)
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

            // Check if item already exists in wishlist
            var existingItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.ProductId == request.ProductId);

            if (existingItem != null)
                return BadRequest(new { message = "Product already in wishlist" });

            // Create new wishlist item
            var wishlistItem = new Wishlist
            {
                UserId = userId.Value,
                ProductId = request.ProductId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Wishlist.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishlistItems), new WishlistItemDto
            {
                Id = wishlistItem.Id,
                UserId = wishlistItem.UserId,
                ProductId = wishlistItem.ProductId,
                CreatedAt = wishlistItem.CreatedAt,
                Product = MapToProductDto(product)
            });
        }

        /// <summary>
        /// Remove item from wishlist
        /// </summary>
        /// <param name="id">Wishlist item ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var wishlistItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId.Value);

            if (wishlistItem == null)
                return NotFound(new { message = "Wishlist item not found" });

            _context.Wishlist.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item removed from wishlist" });
        }

        /// <summary>
        /// Move item from wishlist to cart
        /// </summary>
        /// <param name="id">Wishlist item ID</param>
        /// <param name="request">Move to cart request</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/move-to-cart")]
        public async Task<IActionResult> MoveToCart(int id, [FromBody] MoveToCartDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var wishlistItem = await _context.Wishlist
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId.Value);

            if (wishlistItem == null)
                return NotFound(new { message = "Wishlist item not found" });

            // Check if item already exists in cart
            var existingCartItem = await _context.ShoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId.Value && c.ProductId == wishlistItem.ProductId);

            if (existingCartItem != null)
            {
                // Update quantity in cart
                existingCartItem.Quantity += request.Quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new cart item
                var cartItem = new ShoppingCart
                {
                    UserId = userId.Value,
                    ProductId = wishlistItem.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ShoppingCart.Add(cartItem);
            }

            // Remove from wishlist
            _context.Wishlist.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item moved to cart successfully" });
        }

        /// <summary>
        /// Check if product is in wishlist
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Boolean indicating if product is in wishlist</returns>
        [HttpGet("check/{productId}")]
        public async Task<ActionResult<bool>> IsInWishlist(int productId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var exists = await _context.Wishlist
                .AnyAsync(w => w.UserId == userId.Value && w.ProductId == productId);

            return Ok(exists);
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

    public class AddToWishlistDto
    {
        public int ProductId { get; set; }
    }

    public class MoveToCartDto
    {
        public int Quantity { get; set; } = 1;
    }

    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductDto Product { get; set; } = null!;
    }
}