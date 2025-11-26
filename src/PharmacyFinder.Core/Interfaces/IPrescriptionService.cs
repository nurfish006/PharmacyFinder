using PharmacyFinder.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace PharmacyFinder.Core.Interfaces
{
    // Handles prescription upload, processing, and medicine extraction
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> UploadPrescriptionAsync(PrescriptionUploadDto uploadDto, IFormFile imageFile, string customerId);
        Task<IEnumerable<PrescriptionDto>> GetUserPrescriptionsAsync(string customerId);
        Task<bool> ProcessPrescriptionAsync(int prescriptionId);
        Task<MedicineStockResultDto> SearchFromPrescriptionAsync(int prescriptionId, decimal? latitude, decimal? longitude);
    }
}