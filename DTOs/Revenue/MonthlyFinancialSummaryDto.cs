using Booking_System.DTOs.Expense;

namespace Booking_System.DTOs.Revenue
{
    public class MonthlyFinancialSummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public decimal TotalRevenue { get; set; }          
        public decimal TotalDirectCosts { get; set; }      
        public decimal GrossProfit { get; set; }           
        public decimal TotalExpenses { get; set; }         
        public decimal NetProfit { get; set; }             

        public int ClientCount { get; set; }
        public List<CategoryExpenseSummaryDto> ExpensesByCategory { get; set; } = new();
    }
}
