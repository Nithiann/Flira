using System;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid AttachmentId, string UserId) : IRequest<Result>;
