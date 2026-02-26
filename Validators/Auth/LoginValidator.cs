using Booking_System.DTOs.Auth;
using FluentValidation;

namespace Booking_System.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(UserEmail => UserEmail.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

            RuleFor(UserPassword => UserPassword.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة");
        }
    }
}
