using Microsoft.AspNetCore.Mvc;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Dependency Injection - like requiring services in Express
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);  // Like res.json() in Express
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // 500 Internal Server Error
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);  // 401 Unauthorized
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Get user ID from JWT token (like req.user in Express)
                var userId = User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated");

                var user = await _authService.GetCurrentUserAsync(userId);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}