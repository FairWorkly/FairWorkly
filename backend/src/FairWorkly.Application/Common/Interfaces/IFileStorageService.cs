namespace FairWorkly.Application.Common.Interfaces;

public interface IFileStorageService
{
    /*Files are stored in the wwwroot/uploads folder of the API project*/

    // Upload file to storage and return a unique file path/identifier
    Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default);

    // Get file read stream
    Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken ct = default);
}
