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
                await _authService.RegisterAsync(dto);
                return Ok(new { message = "User registered successfully" });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {

                var token = await _authService.LoginAsync(dto);
                return Ok(new { token });
        }
    }
}
