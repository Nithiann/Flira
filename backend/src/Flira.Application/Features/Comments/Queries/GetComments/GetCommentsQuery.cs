using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Comments.Queries.GetComments;

public record GetCommentsQuery(Guid TaskItemId) : IRequest<Result<List<CommentDto>>>;

public record CommentDto(
    Guid Id,
    Guid TaskItemId,
    string UserId,
    string Username,
    string Content,
    DateTime CreatedAt);
