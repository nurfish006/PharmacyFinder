using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    public interface IMedicineStockRepository : IRepository<MedicineStock>
    {
        Task<MedicineStock> GetByPharmacyAndMedicineAsync(int pharmacyId, int medicineId);
        Task<IEnumerable<MedicineStock>> GetStocksByPharmacyAsync(int pharmacyId);
        Task<IEnumerable<MedicineStock>> SearchStocksByMedicineAsync(string medicineName, decimal? userLat, decimal? userLng, double radiusKm);
        Task UpdateStockQuantityAsync(int pharmacyId, int medicineId, int quantityChange);
    }
}