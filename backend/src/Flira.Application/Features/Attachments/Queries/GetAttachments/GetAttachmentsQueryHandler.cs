using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Features.Attachments.Commands.UploadAttachment;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Attachments.Queries.GetAttachments;

public class GetAttachmentsQueryHandler : IRequestHandler<GetAttachmentsQuery, Result<List<AttachmentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAttachmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AttachmentDto>>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == request.TaskItemId, cancellationToken);
        if (!taskExists)
        {
            return Result.Failure<List<AttachmentDto>>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        var attachments = await _context.Attachments
            .Where(a => a.TaskItemId == request.TaskItemId)
            .Select(a => new AttachmentDto(
                a.Id,
                a.TaskItemId,
                a.FileName,
                a.FileUrl,
                a.FileSizeInBytes,
                a.UploadedById,
                a.CreatedAt
            ))
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result.Success(attachments);
    }
}
