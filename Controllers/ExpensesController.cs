using Booking_System.DTOs;
using Booking_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking_System.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    [Authorize(Roles = "Admin")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var id = await _expenseService.AddExpenseAsync(dto, userId);
            

            return Ok(new { expenseId = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? year = null,
            [FromQuery] int? month = null,
            [FromQuery] string? category = null)
        {
            var expenses = await _expenseService.GetExpensesAsync(year, month, category);
            return Ok(expenses);
        }

        [HttpGet("summary/monthly/{year:int}/{month:int}")]
        public async Task<IActionResult> GetMonthlySummary(int year, int month)
        {
            if (month < 1 || month > 12) return BadRequest("Month must be between 1 and 12");

            var summary = await _expenseService.GetMonthlySummaryAsync(year, month);
            return Ok(summary);
        }
    }
}