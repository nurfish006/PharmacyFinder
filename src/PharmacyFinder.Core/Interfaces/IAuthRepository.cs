using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email);
        Task CreateUserAsync(ApplicationUser user, string password);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    }
}