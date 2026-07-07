using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Search.Queries.SearchTasks;

public class SearchTasksQueryHandler : IRequestHandler<SearchTasksQuery, Result<SearchTasksResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchTasksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SearchTasksResponseDto>> Handle(SearchTasksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TaskItems
            .Include(t => t.BoardColumn)
                .ThenInclude(c => c!.Board)
            .AsNoTracking();

        // Filter by Project
        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.BoardColumn!.Board!.ProjectId == request.ProjectId.Value);
        }

        // Filter by SearchTerm (Title or Description)
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(t => t.Title.Contains(request.SearchTerm) || 
                                     t.Description.Contains(request.SearchTerm));
        }

        // Filter by Assignee
        if (!string.IsNullOrEmpty(request.AssigneeId))
        {
            query = query.Where(t => t.AssigneeId == request.AssigneeId);
        }

        // Filter by Status
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(t => t.Status == request.Status);
        }

        // Filter by Priority
        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }

        // Filter by Labels (multiple comma-separated tags, matching tasks that have all specified tags)
        if (!string.IsNullOrEmpty(request.Labels))
        {
            var filterLabels = request.Labels.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(l => l.Trim().ToLower())
                                             .ToList();

            foreach (var label in filterLabels)
            {
                query = query.Where(t => t.Labels != null && t.Labels.ToLower().Contains(label));
            }
        }

        // Apply Sorting
        var sortExpression = GetSortExpression(request.SortBy);
        var isDescending = request.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

        if (isDescending)
        {
            query = query.OrderByDescending(sortExpression);
        }
        else
        {
            query = query.OrderBy(sortExpression);
        }

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);
        
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new SearchTaskItemDto(
                t.Id,
                t.BoardColumnId,
                t.Title,
                t.Description,
                t.Priority,
                t.Status,
                t.AssigneeId,
                t.ReporterId,
                t.Labels,
                t.DueDate,
                t.EstimatedHours,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var response = new SearchTasksResponseDto(
            items,
            totalCount,
            pageNumber,
            pageSize,
            totalPages
        );

        return Result.Success(response);
    }

    private static Expression<Func<TaskItem, object>> GetSortExpression(string sortBy)
    {
        return (sortBy.ToLower()) switch
        {
            "title" => t => t.Title,
            "duedate" => t => t.DueDate ?? DateTime.MaxValue,
            "priority" => t => t.Priority,
            "createdat" => t => t.CreatedAt,
            _ => t => t.Title
        };
    }
}
