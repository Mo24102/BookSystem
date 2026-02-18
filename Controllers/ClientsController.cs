using Booking_System.DTOs;
using Booking_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking_System.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize]  // All endpoints require authentication
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // ➕ Add Client (User / Admin)
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ClientResponseItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            try
            {
                int clientId = await _clientService.AddClientAsync(dto, userId);
                return Ok(new { message = "Client added successfully", clientId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 📄 Get Clients (Admin sees all / User sees his own)
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? serviceType = null)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);
            bool isAdmin = User.IsInRole("Admin");

            try
            {
                var result = await _clientService.GetClientsAsync(
                    userId: userId,
                    isAdmin: isAdmin,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    search: search,
                    serviceType: serviceType
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── جديد: تعديل عميل (مثل إضافة دفعة جديدة) ──
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientResponseItemDto dto)
        {
            try
            {
                await _clientService.UpdateClientAsync(id, dto);
                return Ok(new { message = "Client updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ❌ Delete Client (Admin only)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _clientService.DeleteClientAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Client not found" });
                }

                return Ok(new { message = "Client deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}