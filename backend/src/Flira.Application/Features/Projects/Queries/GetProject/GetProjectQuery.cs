using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId) : IRequest<Result<ProjectResponseDto>>;

public record ProjectResponseDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Description,
    string Color,
    string Icon,
    DateTime CreatedAt,
    List<BoardDto> Boards);

public record BoardDto(
    Guid Id,
    string Name,
    List<ColumnDto> Columns);

public record ColumnDto(
    Guid Id,
    string Name,
    int Position);
