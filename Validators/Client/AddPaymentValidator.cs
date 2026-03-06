using Booking_System.DTOs.Client;
using FluentValidation;

namespace Booking_System.Validators.Client
{
    public class AddPaymentValidator : AbstractValidator<AddPaymentDto>
    {
        public AddPaymentValidator()
        {
            RuleFor(ClientId => ClientId.ClientId)
                .GreaterThan(0)
                .WithMessage("معرف العميل غير صحيح");

            RuleFor(ClientAmout => ClientAmout.Amount)
                .GreaterThan(0)
                .WithMessage("المبلغ يجب أن يكون أكبر من صفر");
        }
    }
}
