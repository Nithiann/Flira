using FluentValidation;

namespace Flira.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mailadres is verplicht.")
            .EmailAddress().WithMessage("Geef een geldig e-mailadres op.");
    }
}
