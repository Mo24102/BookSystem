namespace Booking_System.DTOs
{
    public class CategoryExpenseSummaryDto
    {
        public string Category { get; set; } = null!;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
