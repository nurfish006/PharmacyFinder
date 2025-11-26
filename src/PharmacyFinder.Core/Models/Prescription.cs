// Models/Prescription.cs
using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.Core.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PatientName { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
        
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        // File storage
        [Required]
        [MaxLength(500)]
        public string ImagePath { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? FileName { get; set; }
        
        public long FileSize { get; set; }
        
        // Status tracking
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Pending;
        
        [MaxLength(1000)]
        public string? ExtractedMedicines { get; set; } // JSON array of medicine names
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        
        // Foreign keys
        public string CustomerId { get; set; } = string.Empty;
        public ApplicationUser Customer { get; set; } = null!;
        
        public int? PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }
    }

    public enum PrescriptionStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Cancelled
    }
}