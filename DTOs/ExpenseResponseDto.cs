namespace Booking_System.DTOs
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = null!;
        public decimal? Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
