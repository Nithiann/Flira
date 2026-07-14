using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flira.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Flira.Infrastructure.Services.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _uploadsFolder;

    public LocalStorageService(IConfiguration configuration)
    {
        var localPath = configuration["StorageSettings:LocalUploadsPath"] 
                        ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        _uploadsFolder = Path.Combine(localPath, "uploads");
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream, cancellationToken);
        }

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_uploadsFolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
