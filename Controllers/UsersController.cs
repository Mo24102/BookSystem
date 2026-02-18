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
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpPut("{id}/make-admin")]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            await _userService.MakeAdminAsync(id);
            return Ok("User promoted to Admin");
        }

        // ── جديد: تعديل مستخدم ──
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User dto)
        {
            try
            {
                await _userService.UpdateUserAsync(id, dto);
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── جديد: حذف مستخدم ──
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                    return NotFound(new { message = "User not found" });
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}