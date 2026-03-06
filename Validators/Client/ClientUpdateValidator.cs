using Booking_System.DTOs.Client;
using FluentValidation;

namespace Booking_System.Validators.Client
{
    public class ClientUpdateValidator : AbstractValidator<ClientUpdateDto>
    {
        public ClientUpdateValidator()
        {
            RuleFor(ClientName => ClientName.ClientName)
                .MinimumLength(3)
                .When(ClientName => !string.IsNullOrEmpty(ClientName.ClientName))
                .WithMessage("اسم العميل يجب أن يكون 3 أحرف على الأقل");

            RuleFor(Clientphone => Clientphone.Phone)
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
                .When(ClientPhone => !string.IsNullOrEmpty(ClientPhone.Phone))
                .WithMessage("رقم الهاتف غير صحيح");

            RuleFor(ClientNationalId => ClientNationalId.NationalId)
                .Length(14)
                .When(ClientNationalId => !string.IsNullOrEmpty(ClientNationalId.NationalId))
                .WithMessage("الرقم القومي يجب أن يكون 14 رقم");

            RuleFor(ClientTotalDua => ClientTotalDua.TotalDue)
                .GreaterThanOrEqualTo(0)
                .WithMessage("إجمالي المبلغ لا يمكن أن يكون أقل من صفر");

            RuleFor(ClientActualCost => ClientActualCost.ActualCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("التكلفة الفعلية لا يمكن أن تكون أقل من صفر");
        }
    }
}
