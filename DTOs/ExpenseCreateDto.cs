namespace Booking_System.DTOs
{
    public class ExpenseCreateDto
    {
        public string Category { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
