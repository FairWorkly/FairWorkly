using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    // Files are stored in the wwwroot/uploads folder of the API project
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public async Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        // Ensure the storage directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        // Generate Unique Filename (Prevent Duplicate Name Overwrite)
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_basePath, uniqueFileName);

        // Write to hard disk
        using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs, ct);

        // Return filename (to be stored in database later)
        return uniqueFileName;
    }

    public Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        // Open file stream return
        return Task.FromResult<Stream?>(File.OpenRead(fullPath));
    }
}