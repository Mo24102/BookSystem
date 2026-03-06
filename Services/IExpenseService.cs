using Booking_System.DTOs.Expense;
using Booking_System.DTOs.Revenue;

namespace Booking_System.Services
{
    public interface IExpenseService
    {
        Task<int> AddExpenseAsync(ExpenseCreateDto dto, int userId);
        Task<List<ExpenseResponseDto>> GetExpensesAsync(int? year = null, int? month = null, string? category = null);
        Task<MonthlyFinancialSummaryDto> GetMonthlySummaryAsync(int year, int month);
        Task UpdateExpenseAsync(int id, ExpenseUpdateDto dto);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
