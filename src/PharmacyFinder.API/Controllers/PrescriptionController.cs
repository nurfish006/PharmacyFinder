using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        // POST /api/prescription/upload
        [HttpPost("upload")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UploadPrescription(
            [FromForm] PrescriptionUploadDto uploadDto,
            [FromForm] IFormFile prescriptionImage)
        {
            try
            {
                var customerId = User.FindFirst("sub")?.Value;
                var result = await _prescriptionService.UploadPrescriptionAsync(uploadDto, prescriptionImage, customerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/prescription/my-prescriptions
        [HttpGet("my-prescriptions")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPrescriptions()
        {
            try
            {
                var customerId = User.FindFirst("sub")?.Value;
                var prescriptions = await _prescriptionService.GetUserPrescriptionsAsync(customerId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST /api/prescription/process/5
        [HttpPost("process/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessPrescription(int id)
        {
            try
            {
                var success = await _prescriptionService.ProcessPrescriptionAsync(id);
                return success ? Ok(new { message = "Prescription processed" }) : BadRequest("Processing failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/prescription/search-from-prescription/5?latitude=40.7128&longitude=-74.0060
        [HttpGet("search-from-prescription/{prescriptionId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SearchFromPrescription(
            int prescriptionId,
            [FromQuery] decimal? latitude,
            [FromQuery] decimal? longitude)
        {
            try
            {
                var result = await _prescriptionService.SearchFromPrescriptionAsync(prescriptionId, latitude, longitude);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}