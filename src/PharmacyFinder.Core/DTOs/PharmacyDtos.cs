using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.Core.DTOs
{
    /// <summary>
    /// WHAT: Data for registering a new pharmacy
    /// WHY: Separates registration data from entity, includes validation
    /// BUSINESS: Requires license number, location coordinates
    /// </summary>
    public class PharmacyRegisterDto
    {
        [Required(ErrorMessage = "Pharmacy name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [MaxLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zip code is required")]
        [MaxLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "License number is required")]
        [MaxLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// WHAT: Pharmacy data for API responses
    /// WHY: Safe data exposure, includes calculated distance for search results
    /// PERFORMANCE: Distance calculated on query, not stored
    /// </summary>
    public class PharmacyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsApproved { get; set; }
        public double? Distance { get; set; } // Calculated distance from user in km
    }
}