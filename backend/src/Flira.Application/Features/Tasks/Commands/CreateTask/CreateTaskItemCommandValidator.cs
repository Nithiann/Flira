using System;
using FluentValidation;

namespace Flira.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    public CreateTaskItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Titel is verplicht.")
            .MaximumLength(200).WithMessage("Titel mag maximaal 200 tekens bevatten.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Vervaldatum mag niet in het verleden liggen.");
    }
}
