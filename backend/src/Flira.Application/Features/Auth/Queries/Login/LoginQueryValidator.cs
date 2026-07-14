using FluentValidation;

namespace Flira.Application.Features.Auth.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mailadres is verplicht.")
            .EmailAddress().WithMessage("Geef een geldig e-mailadres op.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Wachtwoord is verplicht.");
    }
}
