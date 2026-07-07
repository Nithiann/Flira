using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand(Guid TaskItemId, string UserId, string Content) : IRequest<Result<Guid>>;
