using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyFinder.Core.DTOs;
using PharmacyFinder.Core.Interfaces;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET /api/medicine/search?term=paracetamol
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchMedicines([FromQuery] string term)
        {
            try
            {
                var medicines = await _medicineService.SearchMedicinesAsync(term);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/medicine/stock-search?searchTerm=paracetamol&latitude=40.7128&longitude=-74.0060
        [HttpGet("stock-search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchMedicineStock([FromQuery] MedicineSearchDto searchDto)
        {
            try
            {
                var result = await _medicineService.SearchMedicineStockAsync(searchDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/medicine/stock
        [HttpPost("stock")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> UpdateStock([FromBody] StockUpdateDto stockDto)
        {
            try
            {
                // Get pharmacy ID from the authenticated pharmacy owner
                var ownerId = User.FindFirst("sub")?.Value;
                // In real app, you'd get pharmacy ID from owner - for now using first pharmacy
                var pharmacyId = 1; // This should come from user's pharmacy
                
                var success = await _medicineService.UpdateStockAsync(pharmacyId, stockDto);
                return success ? Ok(new { message = "Stock updated successfully" }) : BadRequest("Stock update failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/medicine/pharmacy-stock/5
        [HttpGet("pharmacy-stock/{pharmacyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPharmacyStock(int pharmacyId)
        {
            try
            {
                var stock = await _medicineService.GetPharmacyStockAsync(pharmacyId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/medicine/my-stock
        [HttpGet("my-stock")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> GetMyStock()
        {
            try
            {
                var ownerId = User.FindFirst("sub")?.Value;
                var pharmacyId = 1; // This should come from user's pharmacy
                
                var stock = await _medicineService.GetPharmacyStockAsync(pharmacyId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}