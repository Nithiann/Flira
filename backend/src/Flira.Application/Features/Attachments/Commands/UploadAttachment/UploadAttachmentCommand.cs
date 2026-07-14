using System;
using System.IO;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Attachments.Commands.UploadAttachment;

public record UploadAttachmentCommand(
    Guid TaskItemId,
    string FileName,
    Stream FileStream,
    string ContentType,
    long FileSize,
    string UploadedByUserId) : IRequest<Result<AttachmentDto>>;

public record AttachmentDto(
    Guid Id,
    Guid TaskItemId,
    string FileName,
    string FileUrl,
    long FileSizeInBytes,
    string UploadedById,
    DateTime CreatedAt);
