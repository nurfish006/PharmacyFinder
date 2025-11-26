using Microsoft.AspNetCore.Http;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;
using PharmacyFinder.Core.Models;
using System.Linq; // Added 'using System.Linq' for .Any()

namespace PharmacyFinder.Infrastructure.Services
{
    // Manages prescription processing and medicine extraction
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicineService _medicineService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IMedicineService medicineService,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage)
        {
            _prescriptionRepository = prescriptionRepository;
            _medicineService = medicineService;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        public async Task<PrescriptionDto> UploadPrescriptionAsync(PrescriptionUploadDto uploadDto, IFormFile imageFile, string customerId)
        {
            // Validate file
            // **FIX (Line 31):** Added '||' (OR) operator to connect the two conditions.
            if (imageFile == null || imageFile.Length == 0) 
                throw new ArgumentException("Prescription image is required");

            if (imageFile.Length > 5 * 1024 * 1024) // 5MB limit
                throw new ArgumentException("File size cannot exceed 5MB");

            // Save file
            var filePath = await _fileStorage.SaveFileAsync(imageFile, "prescriptions");

            // Create prescription record
            var prescription = new Prescription
            {
                PatientName = uploadDto.PatientName,
                DateOfBirth = uploadDto.DateOfBirth,
                PhoneNumber = uploadDto.PhoneNumber,
                ImagePath = filePath,
                FileName = imageFile.FileName,
                FileSize = imageFile.Length,
                Status = PrescriptionStatus.Pending,
                Notes = uploadDto.Notes,
                CustomerId = customerId,
                UploadedAt = DateTime.UtcNow
            };

            await _prescriptionRepository.AddAsync(prescription);
            await _unitOfWork.CompleteAsync();

            return MapToDto(prescription);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetUserPrescriptionsAsync(string customerId)
        {
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByUserAsync(customerId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<bool> ProcessPrescriptionAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            if (prescription == null) return false;

            // In a real app, you'd use OCR here to extract medicine names
            // For now, we'll simulate extraction
            var extractedMeds = new[] { "Paracetamol", "Ibuprofen" }; // Simulated OCR result
            
            prescription.ExtractedMedicines = System.Text.Json.JsonSerializer.Serialize(extractedMeds);
            prescription.Status = PrescriptionStatus.Completed;
            prescription.ProcessedAt = DateTime.UtcNow;

            _prescriptionRepository.Update(prescription);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<MedicineStockResultDto> SearchFromPrescriptionAsync(int prescriptionId, decimal? latitude, decimal? longitude)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            // **FIX (Line 87):** Added '||' (OR) operator to connect the two conditions.
            if (prescription == null || string.IsNullOrEmpty(prescription.ExtractedMedicines))
                throw new ArgumentException("Prescription not found or not processed");
                
            // Extract medicine names from JSON
            var medicineNames = System.Text.Json.JsonSerializer.Deserialize<string[]>(prescription.ExtractedMedicines);
            if (medicineNames == null || !medicineNames.Any())
                throw new ArgumentException("No medicines extracted from prescription");

            // Search for the first medicine (in real app, search for all)
            var firstMedicine = medicineNames.First();
            var searchDto = new MedicineSearchDto
            {
                SearchTerm = firstMedicine,
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = 10
            };

            return await _medicineService.SearchMedicineStockAsync(searchDto);
        }

        private PrescriptionDto MapToDto(Prescription prescription)
        {
            string[]? extractedMeds = null;
            if (!string.IsNullOrEmpty(prescription.ExtractedMedicines))
            {
                extractedMeds = System.Text.Json.JsonSerializer.Deserialize<string[]>(prescription.ExtractedMedicines);
            }

            return new PrescriptionDto
            {
                Id = prescription.Id,
                PatientName = prescription.PatientName,
                ImageUrl = $"/uploads/{Path.GetFileName(prescription.ImagePath)}",
                Status = prescription.Status.ToString(),
                UploadedAt = prescription.UploadedAt,
                ExtractedMedicines = extractedMeds
            };
        }
    }
}