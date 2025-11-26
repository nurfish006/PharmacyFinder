using PharmacyFinder.Core.DTOs;

namespace PharmacyFinder.Core.Interfaces
{
    // Business logic for medicine management and search operations
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineDto>> SearchMedicinesAsync(string searchTerm);
        Task<MedicineStockResultDto> SearchMedicineStockAsync(MedicineSearchDto searchDto);
        Task<bool> UpdateStockAsync(int pharmacyId, StockUpdateDto stockDto);
        Task<IEnumerable<MedicineStockDto>> GetPharmacyStockAsync(int pharmacyId);
    }

}