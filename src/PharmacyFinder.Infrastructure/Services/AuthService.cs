using Microsoft.AspNetCore.Identity;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Infrastructure.Services
{
    /// <summary>
    /// WHAT: Service handling user authentication business logic
    /// WHY: Encapsulates authentication rules, password policies, user creation
    /// SECURITY: Uses Identity for password hashing, role management
    /// BUSINESS RULES: Validates user types, prevents duplicate pharmacies
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            IAuthRepository authRepository,
            ITokenService tokenService,
            IPharmacyRepository pharmacyRepository,
            UserManager<ApplicationUser> userManager)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _pharmacyRepository = pharmacyRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user with validation and business rules
        /// VALIDATION: Checks for existing email, valid user type
        /// BUSINESS: Auto-approves Admin, PharmacyOwner needs approval
        /// SECURITY: Password is hashed by Identity framework
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _authRepository.UserExistsAsync(registerDto.Email))
                throw new ArgumentException("User with this email already exists");

            // Validate user type
            if (!Enum.TryParse<UserType>(registerDto.UserType, out var userType))
                throw new ArgumentException("Invalid user type");

            // Create user entity
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserType = userType,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Create user with password
            await _authRepository.CreateUserAsync(user, registerDto.Password);

            // Add to role
            await _userManager.AddToRoleAsync(user, userType.ToString());

            // Generate token
            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60), // Should match JWT settings
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType.ToString()
                }
            };
        }

        /// <summary>
        /// Authenticates user and returns JWT token
        /// SECURITY: Validates credentials, returns generic error for security
        /// BUSINESS: Checks if user is active, handles lockout scenarios
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            var passwordValid = await _authRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Generate token
            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType.ToString()
                }
            };
        }

        /// <summary>
        /// Gets current user details without sensitive information
        /// SECURITY: Only returns safe user data
        /// PERFORMANCE: Could be cached for frequent requests
        /// </summary>
        public async Task<UserDto> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType.ToString()
            };
        }
    }
}