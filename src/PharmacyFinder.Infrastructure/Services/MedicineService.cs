using PharmacyFinder.Core.DTOs;

namespace PharmacyFinder.Core.Interfaces
{
    // Business logic for medicine management and search operations

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