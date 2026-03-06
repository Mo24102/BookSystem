using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_System.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string ClientName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [StringLength(14)]
        public string NationalId { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = null!;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualCost { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDue { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0m;

        [Required]
        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "غير مدفوع";

        public DateTime? LastPaymentDate { get; set; }

        [NotMapped]
        public decimal RemainingAmount => TotalDue - PaidAmount;

        public int? NumberOfInstallments { get; set; }
        public int? PaidInstallments { get; set; }
        public DateTime? FinalDueDate { get; set; }
    }
}
