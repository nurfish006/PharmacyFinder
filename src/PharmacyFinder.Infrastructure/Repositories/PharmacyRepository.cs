using PharmacyFinder.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Models;
using PharmacyFinder.Infrastructure.Data;
using System.Linq; // Ensure this is present for ToList()

namespace PharmacyFinder.Infrastructure.Repositories
{
    public class PharmacyRepository : Repository<Pharmacy>, IPharmacyRepository
    {
        public PharmacyRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Pharmacy>> GetPharmaciesNearLocationAsync(decimal latitude, decimal longitude, double radiusKm)
        {
            // Fix: The ToListAsync() call was on an IOrderedEnumerable (LINQ to Objects)
            // after the AsEnumerable() call, which caused the CS1061 error.
            // We must now force the execution and return the Task.FromResult of the synchronous result.
            
            // 1. Fetch data from DB that meets basic criteria (IsApproved) and pull to memory (ToList()).
            var nearbyPharmacies = await _context.Pharmacies
                .Where(p => p.IsApproved)
                // Note: If the database had spatial support, we would keep this IQueryable
                // and translate CalculateDistance into SQL. Since it doesn't, we pull data:
                .ToListAsync(); // Execute query on the DB

            // 2. Perform distance calculation and ordering in memory (LINQ to Objects).
            var result = nearbyPharmacies
                .Where(p => CalculateDistance(latitude, longitude, p.Latitude, p.Longitude) <= radiusKm)
                .OrderBy(p => CalculateDistance(latitude, longitude, p.Latitude, p.Longitude))
                .ToList(); // Synchronous ToList() since we are in memory

            // We can now return the list.
            return result;
        }

        public async Task<IEnumerable<Pharmacy>> GetPendingApprovalsAsync()
            => await _context.Pharmacies
                .Where(p => !p.IsApproved)
                .Include(p => p.Owner)
                .ToListAsync();

        public async Task<Pharmacy> GetPharmacyWithDetailsAsync(int id)
            => await _context.Pharmacies
                .Include(p => p.Owner)
                .Include(p => p.OperatingHours)
                .Include(p => p.MedicineStocks)
                    .ThenInclude(ms => ms.Medicine)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> PharmacyExistsAsync(string licenseNumber)
            => await _context.Pharmacies.AnyAsync(p => p.LicenseNumber == licenseNumber);

        public async Task<IEnumerable<Pharmacy>> GetPharmaciesByOwnerAsync(string ownerId)
            => await _context.Pharmacies
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371; // Earth radius in km
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