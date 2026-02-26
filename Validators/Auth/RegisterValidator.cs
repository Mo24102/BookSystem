using Booking_System.DTOs.Auth;
using FluentValidation;

namespace Booking_System.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(UserName => UserName.FullName)
                .NotEmpty().WithMessage("الاسم الكامل مطلوب")
                .MinimumLength(3).WithMessage("الاسم يجب أن يكون 3 أحرف على الأقل");

            RuleFor(UserEmail => UserEmail.Email)
                .NotEmpty().WithMessage("الإيميل مطلوب")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح");

            RuleFor(UserPassword => UserPassword.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(6).WithMessage("كلمة المرور يجب ألا تقل عن 6 أحرف")
                .Matches(@"[A-Z]+").WithMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل")
                .Matches(@"[a-z]+").WithMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل")
                .Matches(@"[0-9]+").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل")
                .Matches(@"[\!\?\*\.\&\$\@\%]+").WithMessage("كلمة المرور يجب أن تحتوي على رمز (!? *.) واحد على الأقل");
        }
    }
}
