using FluentValidation;

namespace Flira.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TaskItemId)
            .NotEmpty().WithMessage("TaskItemId is verplicht.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is verplicht.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Reactie-inhoud mag niet leeg zijn.")
            .MaximumLength(2000).WithMessage("Reactie-inhoud mag maximaal 2000 tekens bevatten.");
    }
}
