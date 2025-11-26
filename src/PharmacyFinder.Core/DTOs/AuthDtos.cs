using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.Core.DTOs
{
    /// <summary>
    /// WHAT: Data transfer object for user registration
    /// WHY: Separates API contract from database model, provides validation
    /// SECURITY: Never expose User entity directly to API
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "User type is required")]
        public string UserType { get; set; } = string.Empty; // "Admin", "PharmacyOwner", "Customer"
    }

    /// <summary>
    /// WHAT: Data transfer object for user login
    /// WHY: Separate login contract, allows adding remember me, 2FA in future
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// WHAT: Response object after successful authentication
    /// WHY: Returns token + user info in one response, better client experience
    /// SECURITY: Never return password hashes or sensitive user data
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = null!;
    }

    /// <summary>
    /// WHAT: Safe user data for API responses
    /// WHY: Filters out sensitive data like passwords, email confirmed status, etc.
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string? FullName => $"{FirstName} {LastName}";
    }
}