using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    public interface IPharmacyRepository : IRepository<Pharmacy>
    {
        Task<IEnumerable<Pharmacy>> GetPharmaciesNearLocationAsync(decimal latitude, decimal longitude, double radiusKm);
        Task<IEnumerable<Pharmacy>> GetPendingApprovalsAsync();
        Task<Pharmacy> GetPharmacyWithDetailsAsync(int id);
        Task<bool> PharmacyExistsAsync(string licenseNumber);
        Task<IEnumerable<Pharmacy>> GetPharmaciesByOwnerAsync(string ownerId);
    }
}