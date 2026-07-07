using System;
using System.Threading.Tasks;
using Flira.Application.Features.Search.Queries.SearchTasks;
using Flira.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flira.Api.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> Search([FromQuery] SearchTasksRequestModel model)
    {
        var query = new SearchTasksQuery(
            model.SearchTerm,
            model.ProjectId,
            model.AssigneeId,
            model.Status,
            model.Priority,
            model.Labels,
            model.PageNumber ?? 1,
            model.PageSize ?? 10,
            model.SortBy ?? "Title",
            model.SortOrder ?? "asc"
        );

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(new { ErrorCode = result.Error.Code, Message = result.Error.Message });
        }

        return Ok(result.Value);
    }
}

public record SearchTasksRequestModel(
    string? SearchTerm,
    Guid? ProjectId,
    string? AssigneeId,
    string? Status,
    TaskPriority? Priority,
    string? Labels,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortOrder);
