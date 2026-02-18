namespace Booking_System.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Category { get; set; } = null!;  // Rent, Electricity, Water, Tips, OfficeSupplies, Miscellaneous
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
    }
}