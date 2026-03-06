using Booking_System.DTOs.Client;
using Booking_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking_System.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientCreateDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var clientId = await _clientService.AddClientAsync(dto, userId);

            return Ok(new
            {
                message = "تم إضافة العميل",
                clientId
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? serviceType = null)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            bool isAdmin = User.IsInRole("Admin");

            var result = await _clientService.GetClientsAsync(
                userId,
                isAdmin,
                pageNumber,
                pageSize,
                search,
                serviceType);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ClientUpdateDto dto)
        {
            await _clientService.UpdateClientAsync(id, dto);
            return Ok(new { message = "تم تعديل بيانات العميل" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _clientService.DeleteClientAsync(id);

            if (!deleted)
                return NotFound(new { message = "العميل غير موجود" });

            return Ok(new { message = "تم حذف العميل" });
        }
    }
}
