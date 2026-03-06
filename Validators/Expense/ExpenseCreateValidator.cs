using Booking_System.DTOs.Expense;
using FluentValidation;

namespace Booking_System.Validators.Expense
{
    public class ExpenseCreateValidator : AbstractValidator<ExpenseCreateDto>
    {
        public ExpenseCreateValidator()
        {
            RuleFor(Expense => Expense.Category)
                .NotEmpty().WithMessage("نوع المصروف مطلوب")
                .MinimumLength(2).WithMessage("نوع المصروف غير صحيح");

            RuleFor(Expense => Expense.Amount)
                .GreaterThan(0)
                .WithMessage("قيمة المصروف يجب أن تكون أكبر من صفر");

            RuleFor(Expense => Expense.ExpenseDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("تاريخ المصروف غير صحيح");
        }
    }
}
