using FluentValidation;
using Booking_System.DTOs;

public class ClientCreateValidator : AbstractValidator<ClientResponseItemDto>
{
    public ClientCreateValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^01[0-2,5]{1}[0-9]{8}$");

        RuleFor(x => x.NationalId)
            .Length(14);
    }
}
