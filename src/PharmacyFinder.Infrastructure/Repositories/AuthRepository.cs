using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;
using PharmacyFinder.Infrastructure.Data;

namespace PharmacyFinder.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<bool> UserExistsAsync(string email)
            => await _userManager.FindByEmailAsync(email) != null;

        public async Task CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CheckPasswordAsync(user, password);

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
            => await _userManager.GetRolesAsync(user);
    }
}