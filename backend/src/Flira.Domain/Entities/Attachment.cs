using System;

namespace Flira.Domain.Entities;

public class Attachment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }
    public string UploadedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskItem? TaskItem { get; set; }
}
