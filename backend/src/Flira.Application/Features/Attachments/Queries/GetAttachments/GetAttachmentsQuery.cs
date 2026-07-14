using System;
using System.Collections.Generic;
using Flira.Application.Features.Attachments.Commands.UploadAttachment;
using Flira.Shared;
using MediatR;

namespace Flira.Application.Features.Attachments.Queries.GetAttachments;

public record GetAttachmentsQuery(Guid TaskItemId) : IRequest<Result<List<AttachmentDto>>>;
