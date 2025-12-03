using FairWorkly.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace FairWorkly.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    // Use ContentRootPath for reliability
    public LocalFileStorageService(IHostEnvironment env)
    {
        _basePath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        CancellationToken ct = default
    )
    {
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        // Security: Prevent Path Traversal
        var safeFileName = Path.GetFileName(fileName);

        var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        var fullPath = Path.Combine(_basePath, uniqueFileName);

        // useAsync: true for non-blocking I/O
        await using var fs = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            4096,
            useAsync: true
        );

        // Rewind stream if needed
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        await fileStream.CopyToAsync(fs, ct);

        return uniqueFileName;
    }

    public Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken ct = default)
    {
        // Security: Prevent Path Traversal
        var safeFileName = Path.GetFileName(filePath);
        var fullPath = Path.Combine(_basePath, safeFileName);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        // Async read with shared access
        var fs = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true
        );

        return Task.FromResult<Stream?>(fs);
    }
}
