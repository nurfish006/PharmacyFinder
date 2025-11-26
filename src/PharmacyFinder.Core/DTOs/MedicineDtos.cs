using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.Core.DTOs
{
    // Medicine data for API responses and search operations
    public class MedicineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string? Strength { get; set; }
        public string? Unit { get; set; }
        public bool RequiresPrescription { get; set; }
        public string? Description { get; set; }
    }

    // Data for updating medicine stock in pharmacies
    public class StockUpdateDto
    {
        [Required] public int MedicineId { get; set; }
        [Range(0, int.MaxValue)] public int Quantity { get; set; }
        [Range(0.01, 10000)] public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    // Search parameters for medicine availability
    public class MedicineSearchDto
    {
        public string? SearchTerm { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public double RadiusKm { get; set; } = 10;
        public bool InStockOnly { get; set; } = true;
    }

     // Extended DTOs for medicine stock results
    public class MedicineStockResultDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public IEnumerable<MedicineStockDto> AvailableStocks { get; set; } = new List<MedicineStockDto>();
    }

    public class MedicineStockDto
    {
        public int StockId { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}