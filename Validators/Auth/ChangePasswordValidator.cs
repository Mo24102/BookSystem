using Booking_System.DTOs.Auth;
using FluentValidation;

namespace Booking_System.Validators.Auth
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(UserCurrentPassword => UserCurrentPassword.CurrentPassword)
                .NotEmpty().WithMessage("كلمة المرور الحالية مطلوبة");

            RuleFor(UserNewPassword => UserNewPassword.NewPassword)
                .NotEmpty().WithMessage("كلمة المرور الجديدة مطلوبة")
                .MinimumLength(6).WithMessage("كلمة المرور الجديدة يجب ألا تقل عن 6 أحرف");
        }
    }
}
