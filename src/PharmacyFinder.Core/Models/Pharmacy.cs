using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.Core.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;
        
        // Location for distance calculation
        [Column(TypeName = "decimal(10,6)")]
        public decimal Latitude { get; set; }
        
        [Column(TypeName = "decimal(10,6)")]
        public decimal Longitude { get; set; }
        
        // Additional details
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsApproved { get; set; } = false;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Foreign keys
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
        
        // Navigation properties
        public ICollection<OperatingHour> OperatingHours { get; set; } = new List<OperatingHour>();
        public ICollection<MedicineStock> MedicineStocks { get; set; } = new List<MedicineStock>();
    }

    public class OperatingHour
    {
        public int Id { get; set; }
        
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        
        [Required]
        public TimeSpan OpenTime { get; set; }
        
        [Required]
        public TimeSpan CloseTime { get; set; }
        
        public bool IsClosed { get; set; } = false;
        
        // Foreign key
        public int PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;
    }
}