using Booking_System.DTOs.Expense;
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
        public async Task<IActionResult> Create(ExpenseCreateDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var id = await _expenseService.AddExpenseAsync(dto, userId);

            return Ok(new
            {
                message = "تم إضافة المصروف",
                expenseId = id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int? year = null,
            int? month = null,
            string? category = null)
        {
            var expenses = await _expenseService.GetExpensesAsync(year, month, category);
            return Ok(expenses);
        }

        [HttpGet("summary/monthly/{year:int}/{month:int}")]
        public async Task<IActionResult> GetMonthlySummary(int year, int month)
        {
            if (month < 1 || month > 12)
                return BadRequest("الشهر يجب أن يكون بين 1 و 12");

            var summary = await _expenseService.GetMonthlySummaryAsync(year, month);
            return Ok(summary);
        }
    }
}
