using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Core.Interfaces
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetPrescriptionsByUserAsync(string userId);
        Task<Prescription> GetPrescriptionWithDetailsAsync(int id);
    }
}