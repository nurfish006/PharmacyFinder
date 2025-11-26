using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;
using PharmacyFinder.Infrastructure.Data;

namespace PharmacyFinder.Infrastructure.Repositories
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByUserAsync(string userId)
            => await _context.Prescriptions
                .Where(p => p.CustomerId == userId)
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();

        public async Task<Prescription> GetPrescriptionWithDetailsAsync(int id)
            => await _context.Prescriptions
                .Include(p => p.Customer)
                .Include(p => p.Pharmacy)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}