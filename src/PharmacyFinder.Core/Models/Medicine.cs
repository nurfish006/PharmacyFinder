using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.Core.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? GenericName { get; set; }
        
        [MaxLength(100)]
        public string? Manufacturer { get; set; }
        
        public MedicineForm Form { get; set; }
        
        [MaxLength(50)]
        public string? Strength { get; set; }
        
        [MaxLength(20)]
        public string? Unit { get; set; }
        
        public bool RequiresPrescription { get; set; } = true;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<MedicineStock> MedicineStocks { get; set; } = new List<MedicineStock>();
    }

    // ✅ FIXED: Using surrogate key + unique constraint instead of composite key
    public class MedicineStock
    {
        public int Id { get; set; }
        
        // Foreign keys
        public int PharmacyId { get; set; }
        public int MedicineId { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, 10000.00)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Range(0.01, 10000.00)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public bool IsAvailable { get; set; } = true;
        
        // Batch information
        [MaxLength(50)]
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
        // Navigation properties
        public Pharmacy Pharmacy { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }

    public enum MedicineForm
    {
        Tablet,
        Capsule,
        Syrup,
        Injection,
        Ointment,
        Cream,
        Drops,
        Inhaler,
        Spray,
        Powder,
        Other
    }
}