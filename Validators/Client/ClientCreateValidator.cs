using FluentValidation;
using Booking_System.DTOs.Client;

    public class ClientCreateValidator : AbstractValidator<ClientCreateDto>
    {
        public ClientCreateValidator()
        {
            RuleFor(ClientName => ClientName.ClientName)
                .NotEmpty().WithMessage("اسم العميل مطلوب")
                .MinimumLength(3).WithMessage("اسم العميل يجب أن يكون 3 أحرف على الأقل");

            RuleFor(ClientPhone => ClientPhone.Phone)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب")
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
                .WithMessage("رقم الهاتف غير صحيح");

            RuleFor(ClientNationaID => ClientNationaID.NationalId)
                .Length(14)
                .When(ClientNationaID => !string.IsNullOrEmpty(ClientNationaID.NationalId))
                .WithMessage("الرقم القومي يجب أن يكون 14 رقم");

            RuleFor(ClientTotalDua => ClientTotalDua.TotalDue)
                .GreaterThanOrEqualTo(0)
                .WithMessage("إجمالي المبلغ لا يمكن أن يكون أقل من صفر");

            RuleFor(ClientActualCost => ClientActualCost.ActualCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("التكلفة الفعلية لا يمكن أن تكون أقل من صفر");

            RuleFor(ClientNumberOfInstallments => ClientNumberOfInstallments.NumberOfInstallments)
                .GreaterThan(0)
                .When(ClientNumberOfInstallments => ClientNumberOfInstallments.NumberOfInstallments.HasValue)
                .WithMessage("عدد الأقساط يجب أن يكون أكبر من صفر");
        }
    }

