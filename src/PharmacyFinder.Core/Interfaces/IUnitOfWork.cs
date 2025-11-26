namespace PharmacyFinder.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IPharmacyRepository Pharmacies { get; }
        IMedicineRepository Medicines { get; }
        IMedicineStockRepository MedicineStocks { get; }
        IPrescriptionRepository Prescriptions { get; }
        Task<int> CompleteAsync();
    }
}