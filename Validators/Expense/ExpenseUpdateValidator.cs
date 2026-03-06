using Booking_System.DTOs.Expense;
using FluentValidation;

namespace Booking_System.Validators.Expense
{
    public class ExpenseUpdateValidator : AbstractValidator<ExpenseUpdateDto>
    {
        public ExpenseUpdateValidator()
        {
            RuleFor(Expense => Expense.Category)
                .NotEmpty().WithMessage("نوع المصروف مطلوب");

            RuleFor(Expense => Expense.Amount)
                .GreaterThan(0)
                .WithMessage("قيمة المصروف يجب أن تكون أكبر من صفر");

            RuleFor(Expense => Expense.ExpenseDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("تاريخ المصروف غير صحيح");
        }
    }
}
