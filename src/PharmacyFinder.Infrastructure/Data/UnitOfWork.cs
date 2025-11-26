using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Infrastructure.Repositories;

namespace PharmacyFinder.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public UnitOfWork(ApplicationDbContext context, 
            IAuthRepository authRepository,
            IPharmacyRepository pharmacyRepository,
            IMedicineRepository medicineRepository,
            IMedicineStockRepository medicineStockRepository,
            IPrescriptionRepository prescriptionRepository)
        {
            _context = context;
            Auth = authRepository;
            Pharmacies = pharmacyRepository;
            Medicines = medicineRepository;
            MedicineStocks = medicineStockRepository;
            Prescriptions = prescriptionRepository;
        }

        public IAuthRepository Auth { get; }
        public IPharmacyRepository Pharmacies { get; }
        public IMedicineRepository Medicines { get; }
        public IMedicineStockRepository MedicineStocks { get; }
        public IPrescriptionRepository Prescriptions { get; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}