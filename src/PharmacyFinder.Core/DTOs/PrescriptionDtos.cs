using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.Core.DTOs
{
    // Data for uploading a prescription
    public class PrescriptionUploadDto
    {
        [Required] public string PatientName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
    }

    // Prescription data for API responses
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string[]? ExtractedMedicines { get; set; }
    }
}