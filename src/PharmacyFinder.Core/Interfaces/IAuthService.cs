using PharmacyFinder.Core.DTOs;

namespace PharmacyFinder.Core.Interfaces
{
    /// <summary>
    /// WHAT: Business logic contract for authentication operations
    /// WHY: Separates authentication logic from controllers, enables testing
    /// RESPONSIBILITY: User registration, login, token management
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}