using PharmacyFinder.Core.DTOs;

namespace PharmacyFinder.Core.Interfaces
{
    /// <summary>
    /// WHAT: Business logic contract for pharmacy operations
    /// WHY: Separates pharmacy business rules from controllers
    /// RESPONSIBILITY: Registration, approval, search, stock management
    /// </summary>
    public interface IPharmacyService
    {
        Task<PharmacyDto> RegisterPharmacyAsync(PharmacyRegisterDto pharmacyDto, string ownerId);
        Task<bool> ApprovePharmacyAsync(int pharmacyId, string adminId);
        Task<IEnumerable<PharmacyDto>> GetPharmaciesNearbyAsync(decimal latitude, decimal longitude, double radiusKm);
        Task<IEnumerable<PharmacyDto>> GetPendingApprovalsAsync();
        Task<PharmacyDto> GetPharmacyByIdAsync(int id);
    }
}