using Booking_System.Data;
using Booking_System.DTOs.Expense;
using Booking_System.DTOs.Revenue;
using Booking_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Booking_System.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddExpenseAsync(ExpenseCreateDto dto, int userId)
        {
            var expense = new Expense
            {
                Category = dto.Category,
                Amount = dto.Amount,
                //ExpenseDate = dto.ExpenseDate ,
                Notes = dto.Notes,
                CreatedByUserId = userId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense.Id;
        }

        public async Task<List<ExpenseResponseDto>> GetExpensesAsync(int? year = null, int? month = null, string? category = null)
        {
            var query = _context.Expenses.Include(expense => expense.CreatedByUser).AsQueryable();
            if (year.HasValue) query = query.Where(expense => expense.ExpenseDate.Year == year.Value);
            if (month.HasValue) query = query.Where(expense => expense.ExpenseDate.Month == month.Value);
            if (!string.IsNullOrEmpty(category)) query = query.Where(expense => expense.Category == category);

            return await query
                .OrderByDescending(expense => expense.ExpenseDate)
                .Select(expense => new ExpenseResponseDto
                {
                    Id = expense.Id,
                    Category = expense.Category,
                    Amount = expense.Amount,
                    ExpenseDate = expense.ExpenseDate,
                    Notes = expense.Notes,
                    CreatedBy = expense.CreatedByUser.FullName ?? "Unknown"
                }).ToListAsync();
        }

        public async Task<MonthlyFinancialSummaryDto> GetMonthlySummaryAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

            var clientsQuery = _context.Clients.Where(client => client.CreatedAt >= start && client.CreatedAt < end);

            decimal totalRevenue = await clientsQuery.SumAsync(client => client.PaidAmount);
            decimal totalDirectCosts = await clientsQuery.SumAsync(client => client.ActualCost);
            decimal grossProfit = totalRevenue - totalDirectCosts;

            var expensesQuery = _context.Expenses.Where(expense => expense.ExpenseDate >= start && expense.ExpenseDate < end);
            decimal totalExpenses = await expensesQuery.SumAsync(expense => expense.Amount);
            decimal netProfit = grossProfit - totalExpenses;

            var expensesByCategory = await expensesQuery.GroupBy(expense => expense.Category)
                .Select(Category => new CategoryExpenseSummaryDto
                {
                    Category = Category.Key,
                    Total = Category.Sum(expense => expense.Amount),
                    Count = Category.Count()
                }).ToListAsync();

            return new MonthlyFinancialSummaryDto
            {
                Year = year,
                Month = month,
                TotalRevenue = totalRevenue,
                TotalDirectCosts = totalDirectCosts,
                GrossProfit = grossProfit,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ClientCount = await clientsQuery.CountAsync(),
                ExpensesByCategory = expensesByCategory
            };
        }

        public async Task UpdateExpenseAsync(int id, ExpenseUpdateDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) throw new Exception("المصروف غير موجود");

            if (!string.IsNullOrEmpty(dto.Category)) expense.Category = dto.Category.Trim();
            if (dto.Amount.HasValue) expense.Amount = dto.Amount.Value;
            if (dto.ExpenseDate.HasValue) expense.ExpenseDate = dto.ExpenseDate.Value;
            if (!string.IsNullOrEmpty(dto.Notes)) expense.Notes = dto.Notes.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}