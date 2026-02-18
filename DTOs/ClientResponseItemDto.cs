namespace Booking_System.DTOs
{
    /// <summary>
    /// DTO لعرض بيانات عميل واحد (يُستخدم في قائمة العملاء)
    /// </summary>
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

        // مهم جدًا: هنا حقل الدفعة الإضافية
        public decimal? AdditionalPayment { get; set; }   // الدفعة الجديدة اللي هتتضاف لـ PaidAmount
        // ── الحقول المالية القديمة ──
        public decimal ActualCost { get; set; }        // التكلفة الفعلية على الشركة

        // ── الحقول الجديدة لدعم نظام الدفعات الجزئية ──

        /// <summary>
        /// المبلغ الإجمالي المتفق عليه مع العميل (اللي المفروض يدفعه كله)
        /// </summary>
        public decimal TotalDue { get; set; }

        /// <summary>
        /// المبلغ اللي دفع فعليًا حتى الآن
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// المبلغ المتبقي على العميل (حسابي)
        /// </summary>
        public decimal RemainingAmount { get; set; }

        /// <summary>
        /// حالة الدفع الحالية
        /// أمثلة: "غير مدفوع"، "دفع جزئي"، "مدفوع كامل"، "متأخر"
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// الربح الحالي (بناءً على ما دفع فعليًا)
        /// PaidAmount - ActualCost
        /// </summary>
        public decimal Profit { get; set; }

        // حقول اختيارية (إذا كنت تستخدمها)
        public int? NumberOfInstallments { get; set; }      // عدد الأقساط المتفق عليها
        public int? PaidInstallments { get; set; }          // عدد الأقساط المدفوعة
        public DateTime? LastPaymentDate { get; set; }      // تاريخ آخر دفعة
        public DateTime? FinalDueDate { get; set; }         // الموعد النهائي للتسديد
    }
}