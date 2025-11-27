using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Infrastructure.Services
{
    // Handles pharmacy business logic: registration, approval, and location-based search
    public class PharmacyService : IPharmacyService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PharmacyService(IPharmacyRepository pharmacyRepository, IUnitOfWork unitOfWork)
        {
            _pharmacyRepository = pharmacyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PharmacyDto> RegisterPharmacyAsync(PharmacyRegisterDto pharmacyDto, string ownerId)
        {
            // Check for duplicate license number
            if (await _pharmacyRepository.PharmacyExistsAsync(pharmacyDto.LicenseNumber))
                throw new InvalidOperationException("Pharmacy with this license number already exists");

            var pharmacy = new Pharmacy
            {
                Name = pharmacyDto.Name,
                Address = pharmacyDto.Address,
                City = pharmacyDto.City,
                State = pharmacyDto.State,
                ZipCode = pharmacyDto.ZipCode,
                PhoneNumber = pharmacyDto.PhoneNumber,
                Email = pharmacyDto.Email,
                LicenseNumber = pharmacyDto.LicenseNumber,
                Latitude = pharmacyDto.Latitude,
                Longitude = pharmacyDto.Longitude,
                Description = pharmacyDto.Description,
                OwnerId = ownerId,
                IsApproved = false, // Requires admin approval
                RegisteredAt = DateTime.UtcNow
            };

            await _pharmacyRepository.AddAsync(pharmacy);
            await _unitOfWork.CompleteAsync();

            return MapToDto(pharmacy);
        }

        public async Task<bool> ApprovePharmacyAsync(int pharmacyId, string adminId)
        {
            var pharmacy = await _pharmacyRepository.GetByIdAsync(pharmacyId);
            if (pharmacy == null) throw new ArgumentException("Pharmacy not found");

            pharmacy.IsApproved = true;
            pharmacy.UpdatedAt = DateTime.UtcNow;
            
            _pharmacyRepository.Update(pharmacy);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<PharmacyDto>> GetPharmaciesNearbyAsync(decimal latitude, decimal longitude, double radiusKm)
        {
            var pharmacies = await _pharmacyRepository.GetPharmaciesNearLocationAsync(latitude, longitude, radiusKm);
            return pharmacies.Select(MapToDto);
        }

        public async Task<IEnumerable<PharmacyDto>> GetPendingApprovalsAsync()
        {
            var pharmacies = await _pharmacyRepository.GetPendingApprovalsAsync();
            return pharmacies.Select(MapToDto);
        }

        public async Task<PharmacyDto> GetPharmacyByIdAsync(int id)
        {
            var pharmacy = await _pharmacyRepository.GetPharmacyWithDetailsAsync(id);
            if (pharmacy == null) throw new ArgumentException("Pharmacy not found");

            return MapToDto(pharmacy);
        }

        public async Task<IEnumerable<PharmacyDto>> GetPharmaciesByOwnerAsync(string ownerId)
        {
            var pharmacies = await _pharmacyRepository.GetPharmaciesByOwnerAsync(ownerId);
            return pharmacies.Select(MapToDto);
        }

        private PharmacyDto MapToDto(Pharmacy pharmacy) => new()
        {
            Id = pharmacy.Id,
            Name = pharmacy.Name,
            Address = pharmacy.Address,
            City = pharmacy.City,
            State = pharmacy.State,
            ZipCode = pharmacy.ZipCode,
            PhoneNumber = pharmacy.PhoneNumber,
            Email = pharmacy.Email,
            Latitude = pharmacy.Latitude,
            Longitude = pharmacy.Longitude,
            IsApproved = pharmacy.IsApproved
        };
    }
}