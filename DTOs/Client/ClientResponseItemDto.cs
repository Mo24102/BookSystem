namespace Booking_System.DTOs.Client
{
    public class ClientResponseItemDto
    {
        public int Id { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal ActualCost { get; set; }
        public decimal TotalDue { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Profit { get; set; }
        public int? NumberOfInstallments { get; set; }
        public int? PaidInstallments { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? FinalDueDate { get; set; }
    }
}