using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Like Express middleware that checks JWT
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        // POST /api/pharmacy/register
        [HttpPost("register")]
        [Authorize(Roles = "PharmacyOwner")]  // Only PharmacyOwner can access
        public async Task<IActionResult> RegisterPharmacy(PharmacyRegisterDto pharmacyDto)
        {
            try
            {
                var ownerId = User.FindFirst("sub")?.Value;  // Get user ID from JWT
                var result = await _pharmacyService.RegisterPharmacyAsync(pharmacyDto, ownerId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/pharmacy/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10
        [HttpGet("nearby")]
        [AllowAnonymous]  // Public endpoint - no authentication required
        public async Task<IActionResult> GetNearbyPharmacies(
            [FromQuery] decimal latitude, 
            [FromQuery] decimal longitude, 
            [FromQuery] double radiusKm = 10)
        {
            try
            {
                var pharmacies = await _pharmacyService.GetPharmaciesNearbyAsync(latitude, longitude, radiusKm);
                return Ok(pharmacies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/pharmacy/pending-approvals
        [HttpGet("pending-approvals")]
        [Authorize(Roles = "Admin")]  // Only Admin can access
        public async Task<IActionResult> GetPendingApprovals()
        {
            var pharmacies = await _pharmacyService.GetPendingApprovalsAsync();
            return Ok(pharmacies);
        }

        // PUT /api/pharmacy/approve/5
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePharmacy(int id)
        {
            try
            {
                var adminId = User.FindFirst("sub")?.Value;
                var success = await _pharmacyService.ApprovePharmacyAsync(id, adminId);
                return success ? Ok(new { message = "Pharmacy approved" }) : BadRequest("Approval failed");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET /api/pharmacy/my-pharmacies
        [HttpGet("my-pharmacies")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> GetMyPharmacies()
        {
            try
            {
                var ownerId = User.FindFirst("sub")?.Value;
                var pharmacies = await _pharmacyService.GetPharmaciesByOwnerAsync(ownerId);
                return Ok(pharmacies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/pharmacy/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPharmacyById(int id)
        {
            try
            {
                var pharmacy = await _pharmacyService.GetPharmacyByIdAsync(id);
                return Ok(pharmacy);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}