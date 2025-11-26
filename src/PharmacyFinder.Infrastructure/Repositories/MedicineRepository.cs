using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;
using PharmacyFinder.Infrastructure.Data;

namespace PharmacyFinder.Infrastructure.Repositories
{
    public class MedicineRepository : Repository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _context.Medicines
                .Where(m => m.Name.Contains(searchTerm) || 
                           (m.GenericName != null && m.GenericName.Contains(searchTerm)) ||
                           m.Manufacturer.Contains(searchTerm))
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        public async Task<Medicine> GetMedicineWithStocksAsync(int id)
            => await _context.Medicines
                .Include(m => m.MedicineStocks)
                    .ThenInclude(ms => ms.Pharmacy)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<bool> MedicineExistsAsync(string name, string manufacturer)
            => await _context.Medicines.AnyAsync(m => 
                m.Name == name && m.Manufacturer == manufacturer);
    }
}