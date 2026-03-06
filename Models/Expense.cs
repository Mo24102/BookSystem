using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_System.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
    }
}