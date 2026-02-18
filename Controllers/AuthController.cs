using Booking_System.DTOs;
using Booking_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking_System.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                await _authService.RegisterAsync(dto);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                // هنا بناخد الرسالة اللي في الـ throw ونرجعها كـ BadRequest
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
