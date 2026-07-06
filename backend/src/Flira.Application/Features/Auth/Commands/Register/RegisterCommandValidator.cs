using FluentValidation;

namespace Flira.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mailadres is verplicht.")
            .EmailAddress().WithMessage("Geef een geldig e-mailadres op.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Wachtwoord is verplicht.")
            .MinimumLength(6).WithMessage("Wachtwoord moet minimaal 6 tekens bevatten.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Volledige naam is verplicht.")
            .MaximumLength(100).WithMessage("Naam mag maximaal 100 tekens bevatten.");
    }
}
