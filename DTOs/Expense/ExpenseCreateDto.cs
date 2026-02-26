namespace Booking_System.DTOs.Expense
{
    public class ExpenseCreateDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
    }
}
