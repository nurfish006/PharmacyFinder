using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;
using PharmacyFinder.Infrastructure.Data;

namespace PharmacyFinder.Infrastructure.Repositories
{
    public class MedicineStockRepository : Repository<MedicineStock>, IMedicineStockRepository
    {
        public MedicineStockRepository(ApplicationDbContext context) : base(context) { }

        public async Task<MedicineStock> GetByPharmacyAndMedicineAsync(int pharmacyId, int medicineId)
            => await _context.MedicineStocks
                .Include(ms => ms.Medicine)
                .Include(ms => ms.Pharmacy)
                .FirstOrDefaultAsync(ms => ms.PharmacyId == pharmacyId && ms.MedicineId == medicineId);

        public async Task<IEnumerable<MedicineStock>> GetStocksByPharmacyAsync(int pharmacyId)
            => await _context.MedicineStocks
                .Include(ms => ms.Medicine)
                .Where(ms => ms.PharmacyId == pharmacyId && ms.Quantity > 0)
                .ToListAsync();

        public async Task<IEnumerable<MedicineStock>> SearchStocksByMedicineAsync(string medicineName, decimal? userLat, decimal? userLng, double radiusKm)
        {
            var query = _context.MedicineStocks
                .Include(ms => ms.Medicine)
                .Include(ms => ms.Pharmacy)
                .Where(ms => ms.Quantity > 0 && ms.Pharmacy.IsApproved)
                .Where(ms => ms.Medicine.Name.Contains(medicineName) || 
                            (ms.Medicine.GenericName != null && ms.Medicine.GenericName.Contains(medicineName)));

            // If location provided, filter by distance
            if (userLat.HasValue && userLng.HasValue)
            {
                var stocks = await query.ToListAsync();
                return stocks
                    .Where(ms => CalculateDistance(userLat.Value, userLng.Value, ms.Pharmacy.Latitude, ms.Pharmacy.Longitude) <= radiusKm)
                    .OrderBy(ms => CalculateDistance(userLat.Value, userLng.Value, ms.Pharmacy.Latitude, ms.Pharmacy.Longitude))
                    .ThenBy(ms => ms.Price);
            }

            return await query
                .OrderBy(ms => ms.Price)
                .ToListAsync();
        }

        public async Task UpdateStockQuantityAsync(int pharmacyId, int medicineId, int quantityChange)
        {
            var stock = await GetByPharmacyAndMedicineAsync(pharmacyId, medicineId);
            if (stock != null)
            {
                stock.Quantity += quantityChange;
                stock.LastUpdated = DateTime.UtcNow;
                stock.IsAvailable = stock.Quantity > 0;
                Update(stock);
            }
        }

        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371;
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}