using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Domain.Entities;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Attachments.Commands.UploadAttachment;

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Result<AttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStorageService _storageService;

    private static readonly string[] UnsafeExtensions = { ".exe", ".bat", ".sh", ".cmd", ".com", ".msi", ".scr", ".vbs", ".ps1" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public UploadAttachmentCommandHandler(
        IApplicationDbContext context,
        IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<Result<AttachmentDto>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == request.TaskItemId, cancellationToken);
        if (!taskExists)
        {
            return Result.Failure<AttachmentDto>(new Error("Task.NotFound", "Taak niet gevonden."));
        }

        // Validate File Size
        if (request.FileSize > MaxFileSizeBytes)
        {
            return Result.Failure<AttachmentDto>(new Error("Attachment.TooLarge", "De maximale bestandsgrootte is 10MB."));
        }

        // Validate File Extension
        var extension = Path.GetExtension(request.FileName).ToLower();
        if (UnsafeExtensions.Contains(extension))
        {
            return Result.Failure<AttachmentDto>(new Error("Attachment.UnsafeType", $"Het bestandstype '{extension}' is niet toegestaan om veiligheidsredenen."));
        }

        // Upload to Storage
        string fileUrl;
        try
        {
            fileUrl = await _storageService.UploadAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            return Result.Failure<AttachmentDto>(new Error("Storage.UploadFailed", $"Uploaden mislukt: {ex.Message}"));
        }

        // Save metadata in database
        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            TaskItemId = request.TaskItemId,
            FileName = request.FileName,
            FileUrl = fileUrl,
            FileSizeInBytes = request.FileSize,
            UploadedById = request.UploadedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new AttachmentDto(
            attachment.Id,
            attachment.TaskItemId,
            attachment.FileName,
            attachment.FileUrl,
            attachment.FileSizeInBytes,
            attachment.UploadedById,
            attachment.CreatedAt
        );

        return Result.Success(dto);
    }
}
