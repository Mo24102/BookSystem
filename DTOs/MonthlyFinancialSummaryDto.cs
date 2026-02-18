namespace Booking_System.DTOs
{
    public class MonthlyFinancialSummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public decimal TotalRevenue { get; set; }          // مجموع PaidAmount
        public decimal TotalDirectCosts { get; set; }      // مجموع ActualCost
        public decimal GrossProfit { get; set; }           // Revenue - DirectCosts
        public decimal TotalExpenses { get; set; }         // مصروفات عامة
        public decimal NetProfit { get; set; }             // GrossProfit - Expenses

        public int ClientCount { get; set; }
        public List<CategoryExpenseSummaryDto> ExpensesByCategory { get; set; } = new();
    }
}
