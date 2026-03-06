using Booking_System.DTOs.Auth;
using Booking_System.Models;
using Booking_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking_System.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}/make-admin")]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            await _userService.MakeAdminAsync(id);
            return Ok(new { message = "تم ترقية المستخدم إلى أدمن" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegisterDto dto)
        {
            await _userService.UpdateUserAsync(id, dto);
            return Ok(new { message = "تم تعديل المستخدم بنجاح" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
                return NotFound(new { message = "المستخدم غير موجود" });

            return Ok(new { message = "تم حذف المستخدم" });
        }
    }
}
