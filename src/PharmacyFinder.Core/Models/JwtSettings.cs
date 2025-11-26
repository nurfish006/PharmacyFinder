namespace PharmacyFinder.Core.Models
{
    /// <summary>
    /// Configuration model for JWT (JSON Web Token) settings.
    /// This is loaded from appsettings.json via dependency injection.
    /// </summary>
    public class JwtSettings
    {
        // This key must be a secure, long, secret string (at least 16 characters).
        public string Secret { get; set; } = string.Empty; 
        
        // Who issues the token (e.g., your application domain).
        public string Issuer { get; set; } = string.Empty;

        // Who the token is for (e.g., your application domain).
        public string Audience { get; set; } = string.Empty;

        // How long the token is valid, in minutes.
        public int ExpiryInMinutes { get; set; } = 60;
    }
}