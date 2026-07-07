using System;
using System.Collections.Generic;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(Guid ProjectId) : IRequest<Result<DashboardStatsResponse>>;

public record DashboardStatsResponse(
    int OpenTasksCount,
    int ClosedTasksCount,
    decimal TotalCompletedHours,
    List<BurndownDayDto> BurndownData);

public record BurndownDayDto(
    DateTime Date,
    int CompletedTasksCount,
    int RemainingTasksCount);
