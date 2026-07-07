using System;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Flira.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flira.Application.Features.Attachments.Commands.DeleteAttachment;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IStorageService _storageService;

    public DeleteAttachmentCommandHandler(
        IApplicationDbContext context,
        IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId, cancellationToken);

        if (attachment == null)
        {
            return Result.Failure(new Error("Attachment.NotFound", "Bijlage niet gevonden."));
        }

        // Ownership validation
        if (attachment.UploadedById != request.UserId)
        {
            return Result.Failure(new Error("Attachment.Forbidden", "Je bent niet de eigenaar van deze bijlage."));
        }

        // Delete from storage
        try
        {
            await _storageService.DeleteAsync(attachment.FileUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            // We can log this but still proceed to remove database record so it doesn't get orphaned/stuck.
            Console.WriteLine($"Storage deletion failed for {attachment.FileUrl}: {ex.Message}");
        }

        // Delete from database
        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
