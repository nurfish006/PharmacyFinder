using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;

namespace PharmacyFinder.Infrastructure.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMedicineStockRepository _medicineStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MedicineService(
            IMedicineRepository medicineRepository,
            IMedicineStockRepository medicineStockRepository,
            IUnitOfWork unitOfWork)
        {
            _medicineRepository = medicineRepository;
            _medicineStockRepository = medicineStockRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MedicineDto>> SearchMedicinesAsync(string searchTerm)
        {
            var medicines = await _medicineRepository.SearchMedicinesAsync(searchTerm);
            return medicines.Select(MapToDto);
        }

        public async Task<MedicineStockResultDto> SearchMedicineStockAsync(MedicineSearchDto searchDto)
        {
            if (string.IsNullOrWhiteSpace(searchDto.SearchTerm))
                throw new ArgumentException("Search term is required");

            var stocks = await _medicineStockRepository.SearchStocksByMedicineAsync(
                searchDto.SearchTerm,
                searchDto.Latitude,
                searchDto.Longitude,
                searchDto.RadiusKm);

            var stockDtos = stocks.Select(s => new MedicineStockDto
            {
                StockId = s.Id,
                PharmacyName = s.Pharmacy.Name,
                Address = $"{s.Pharmacy.Address}, {s.Pharmacy.City}, {s.Pharmacy.State}",
                Distance = 0,
                Quantity = s.Quantity,
                Price = s.Price,
                DiscountPrice = s.DiscountPrice,
                LastUpdated = s.LastUpdated
            }).ToList();

            return new MedicineStockResultDto
            {
                MedicineName = searchDto.SearchTerm,
                AvailableStocks = stockDtos
            };
        }

        public async Task<bool> UpdateStockAsync(int pharmacyId, StockUpdateDto stockDto)
        {
            var stock = await _medicineStockRepository.GetByPharmacyAndMedicineAsync(pharmacyId, stockDto.MedicineId);

            if (stock == null)
            {
                stock = new MedicineStock
                {
                    PharmacyId = pharmacyId,
                    MedicineId = stockDto.MedicineId,
                    Quantity = stockDto.Quantity,
                    Price = stockDto.Price,
                    DiscountPrice = stockDto.DiscountPrice,
                    BatchNumber = stockDto.BatchNumber,
                    ExpiryDate = stockDto.ExpiryDate,
                    LastUpdated = DateTime.UtcNow,
                    IsAvailable = stockDto.Quantity > 0
                };
                await _medicineStockRepository.AddAsync(stock);
            }
            else
            {
                stock.Quantity = stockDto.Quantity;
                stock.Price = stockDto.Price;
                stock.DiscountPrice = stockDto.DiscountPrice;
                stock.BatchNumber = stockDto.BatchNumber;
                stock.ExpiryDate = stockDto.ExpiryDate;
                stock.LastUpdated = DateTime.UtcNow;
                stock.IsAvailable = stockDto.Quantity > 0;
                _medicineStockRepository.Update(stock);
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<MedicineStockDto>> GetPharmacyStockAsync(int pharmacyId)
        {
            var stocks = await _medicineStockRepository.GetStocksByPharmacyAsync(pharmacyId);

            return stocks.Select(s => new MedicineStockDto
            {
                StockId = s.Id,
                PharmacyName = s.Pharmacy?.Name ?? "Unknown",
                Address = s.Pharmacy != null ? $"{s.Pharmacy.Address}, {s.Pharmacy.City}" : "Unknown",
                Distance = 0,
                Quantity = s.Quantity,
                Price = s.Price,
                DiscountPrice = s.DiscountPrice,
                LastUpdated = s.LastUpdated
            });
        }

        private MedicineDto MapToDto(Medicine medicine) => new()
        {
            Id = medicine.Id,
            Name = medicine.Name,
            GenericName = medicine.GenericName,
            Manufacturer = medicine.Manufacturer,
            Form = medicine.Form.ToString(),
            Strength = medicine.Strength,
            Unit = medicine.Unit,
            RequiresPrescription = medicine.RequiresPrescription,
            Description = medicine.Description
        };
    }
}
