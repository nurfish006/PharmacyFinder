using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    public interface IMedicineRepository : IRepository<Medicine>
    {
        Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
        Task<Medicine> GetMedicineWithStocksAsync(int id);
        Task<bool> MedicineExistsAsync(string name, string manufacturer);
    }
}