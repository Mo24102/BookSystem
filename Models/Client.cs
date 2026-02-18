namespace Booking_System.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string ServiceType { get; set; } = null!;
        public string Notes { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── الحقول المالية القديمة (التي كانت موجودة) ──
        public decimal ActualCost { get; set; } = 0m;     // التكلفة الفعلية على الشركة

        // ── الحقول الجديدة لدعم الدفعات الجزئية ──

        /// <summary>
        /// المبلغ الإجمالي المتفق عليه مع العميل (اللي لازم يدفعه كامل)
        /// مثال: 10000 جنيه
        /// </summary>
        public decimal TotalDue { get; set; } = 0m;

        /// <summary>
        /// المبلغ اللي دفع فعليًا حتى الآن (الدفعة الأولى + أي دفعات لاحقة)
        /// مثال: 2000 جنيه
        /// </summary>
        public decimal PaidAmount { get; set; } = 0m;

        /// <summary>
        /// حالة الدفع الحالية للعميل
        /// القيم المقترحة: 
        /// "غير مدفوع" | "دفع جزئي" | "مدفوع كامل" | "متأخر"
        /// </summary>
        public string PaymentStatus { get; set; } = "غير مدفوع";

        /// <summary>
        /// تاريخ آخر دفعة تم تسجيلها (مفيد لمعرفة التأخير)
        /// </summary>
        public DateTime? LastPaymentDate { get; set; }

        /// <summary>
        /// المبلغ المتبقي على العميل (حسابي - مش لازم تخزنه، بس ممكن تحسبه)
        /// </summary>
        public decimal RemainingAmount => TotalDue - PaidAmount;

        // حقول اختيارية (لو عايز تدعم نظام أقساط بشكل أوضح)
        public int? NumberOfInstallments { get; set; }     // عدد الأقساط المتفق عليها
        public int? PaidInstallments { get; set; }         // عدد الأقساط المدفوعة
        public DateTime? FinalDueDate { get; set; }        // الموعد النهائي لتسديد كامل المبلغ
    }
}