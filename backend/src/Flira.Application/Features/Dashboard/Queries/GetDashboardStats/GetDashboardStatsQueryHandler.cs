using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardStatsResponse>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            return Result.Failure<DashboardStatsResponse>(new Error("Project.NotFound", "Project niet gevonden."));
        }

        // Read-Optimized queries using AsNoTracking()
        var tasks = await _context.TaskItems
            .Include(t => t.BoardColumn)
                .ThenInclude(c => c!.Board)
            .Where(t => t.BoardColumn!.Board!.ProjectId == request.ProjectId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var openTasksCount = tasks.Count(t => !t.Status.Equals("Done", StringComparison.OrdinalIgnoreCase));
        var closedTasksCount = tasks.Count(t => t.Status.Equals("Done", StringComparison.OrdinalIgnoreCase));

        // Team velocity: Completed hours in the last 30 days
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var totalCompletedHours = tasks
            .Where(t => t.Status.Equals("Done", StringComparison.OrdinalIgnoreCase) && 
                        t.CompletedAt.HasValue && 
                        t.CompletedAt.Value >= thirtyDaysAgo)
            .Sum(t => t.EstimatedHours ?? 0);

        // Generate burndown data for the last 30 days
        var burndownData = new List<BurndownDayDto>();
        var startDate = DateTime.UtcNow.Date.AddDays(-29); // include today (total 30 days)

        for (int i = 0; i < 30; i++)
        {
            var date = startDate.AddDays(i);

            // Tasks created up to the end of this date
            var createdUpToDate = tasks.Count(t => t.CreatedAt.Date <= date);

            // Tasks completed up to the end of this date
            var completedUpToDate = tasks.Count(t => 
                t.Status.Equals("Done", StringComparison.OrdinalIgnoreCase) && 
                t.CompletedAt.HasValue && 
                t.CompletedAt.Value.Date <= date
            );

            // Tasks completed specifically on this date
            var completedOnDate = tasks.Count(t => 
                t.Status.Equals("Done", StringComparison.OrdinalIgnoreCase) && 
                t.CompletedAt.HasValue && 
                t.CompletedAt.Value.Date == date
            );

            var remainingTasksCount = createdUpToDate - completedUpToDate;

            burndownData.Add(new BurndownDayDto(
                date,
                completedOnDate,
                remainingTasksCount
            ));
        }

        var response = new DashboardStatsResponse(
            openTasksCount,
            closedTasksCount,
            totalCompletedHours,
            burndownData
        );

        return Result.Success(response);
    }
}
