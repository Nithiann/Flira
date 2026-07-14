using FluentValidation;

namespace Flira.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mailadres is verplicht.")
            .EmailAddress().WithMessage("Geef een geldig e-mailadres op.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is verplicht.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nieuw wachtwoord is verplicht.")
            .MinimumLength(6).WithMessage("Nieuw wachtwoord moet minimaal 6 tekens bevatten.");
    }
}
