using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(Guid OrganizationId) : IRequest<Result<List<ProjectDto>>>;

public record ProjectDto(Guid Id, Guid OrganizationId, string Name, string Description, string Color, string Icon);
