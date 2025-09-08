using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShopForHome.API.DTOs;
using ShopForHome.API.Services;

namespace ShopForHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto">Registration details</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _authService.EmailExistsAsync(registerDto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return BadRequest(new { message = "Registration failed" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="updateProfileDto">Updated profile information</param>
        /// <returns>Success status</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _authService.UpdateUserAsync(userId.Value, updateProfileDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to update profile" });
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordDto">Password change details</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please check your current password." });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>Boolean indicating if email exists</returns>
        [HttpGet("check-email")]
        public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var exists = await _authService.EmailExistsAsync(email);
            return Ok(new { exists });
        }

        /// <summary>
        /// Refresh token (placeholder for future implementation)
        /// </summary>
        /// <returns>New JWT token</returns>
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return Unauthorized();
            }

            // For now, we'll just generate a new token
            // In a production app, you'd want to implement proper refresh token logic
            var userEntity = new Models.User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };

            var token = _authService.GenerateJwtToken(userEntity);
            var expiration = DateTime.UtcNow.AddMinutes(60);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                User = user
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}