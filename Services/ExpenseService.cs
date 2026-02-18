using Booking_System.Data;
using Booking_System.DTOs;
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
                ExpenseDate = dto.ExpenseDate ?? DateTime.UtcNow,
                Notes = dto.Notes,
                CreatedByUserId = userId
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense.Id;
        }

        public async Task<List<ExpenseResponseDto>> GetExpensesAsync(int? year = null, int? month = null, string? category = null)
        {
            var query = _context.Expenses
                .Include(e => e.CreatedByUser)
                .AsQueryable();

            if (year.HasValue) query = query.Where(e => e.ExpenseDate.Year == year.Value);
            if (month.HasValue) query = query.Where(e => e.ExpenseDate.Month == month.Value);
            if (!string.IsNullOrEmpty(category)) query = query.Where(e => e.Category == category);

            return await query
                .OrderByDescending(e => e.ExpenseDate)
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Category = e.Category,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    Notes = e.Notes,
                    CreatedBy = e.CreatedByUser == null ? "Unknown" : e.CreatedByUser.FullName
                })
                .ToListAsync();
        }

        public async Task<MonthlyFinancialSummaryDto> GetMonthlySummaryAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

            var clientsQuery = _context.Clients
                .Where(c => c.CreatedAt >= start && c.CreatedAt < end);

            decimal totalRevenue = await clientsQuery.SumAsync(c => c.PaidAmount);
            decimal totalDirectCosts = await clientsQuery.SumAsync(c => c.ActualCost);
            decimal grossProfit = totalRevenue - totalDirectCosts;

            var expensesQuery = _context.Expenses
                .Where(e => e.ExpenseDate >= start && e.ExpenseDate < end);

            decimal totalExpenses = await expensesQuery.SumAsync(e => e.Amount);
            decimal netProfit = grossProfit - totalExpenses;

            var expensesByCategory = await expensesQuery
                .GroupBy(e => e.Category)
                .Select(g => new CategoryExpenseSummaryDto
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count()
                })
                .ToListAsync();

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

        public async Task UpdateExpenseAsync(int id, ExpenseResponseDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                throw new Exception("Expense not found");

            if (!string.IsNullOrEmpty(dto.Category))
                expense.Category = dto.Category.Trim();

            if (dto.Amount.HasValue)
                expense.Amount = dto.Amount.Value;

            if (dto.ExpenseDate.HasValue)
                expense.ExpenseDate = dto.ExpenseDate.Value;

            if (!string.IsNullOrEmpty(dto.Notes))
                expense.Notes = dto.Notes.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}