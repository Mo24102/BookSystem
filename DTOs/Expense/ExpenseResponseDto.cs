namespace Booking_System.DTOs.Expense
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
