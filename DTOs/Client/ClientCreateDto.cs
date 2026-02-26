namespace Booking_System.DTOs.Client
{
    public class ClientCreateDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }
        public decimal ActualCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalDue { get; set; }
        public int? NumberOfInstallments { get; set; }
        public DateTime? FinalDueDate { get; set; }
    }
}
