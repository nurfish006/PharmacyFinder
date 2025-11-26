using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    /// <summary>
    /// WHAT: Service contract for JWT token operations
    /// WHY: Abstraction allows changing token implementation without affecting other code
    /// SECURITY: Separates token generation logic from business services
    /// </summary>
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
        string? ValidateToken(string token);
    }
}