namespace Booking_System.DTOs.Expense
{
    public class CategoryExpenseSummaryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
