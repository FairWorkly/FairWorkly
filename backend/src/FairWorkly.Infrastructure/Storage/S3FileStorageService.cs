using Amazon.S3;
using Amazon.S3.Model;
using FairWorkly.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Infrastructure.Storage;

/// <summary>
/// AWS S3 implementation of file storage service.
/// Files are stored in S3 with organized key structure: rosters/yyyy/MM/dd/guid/filename
///
/// Required NuGet packages (to be installed by DevOps):
/// - AWSSDK.S3
/// - AWSSDK.Extensions.NETCore.Setup
///
/// Required configuration in appsettings.json:
/// {
///   "AWS": {
///     "Region": "ap-southeast-2",
///     "S3": {
///       "BucketName": "fairworkly-roster-files",
///       "RosterFilesPrefix": "rosters/"
///     }
///   }
/// }
/// </summary>
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _rosterFilesPrefix;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(
        IAmazonS3 s3Client,
        IConfiguration configuration,
        ILogger<S3FileStorageService> logger
    )
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _bucketName =
            configuration["AWS:S3:BucketName"]
            ?? throw new InvalidOperationException("AWS:S3:BucketName configuration is missing");

        _rosterFilesPrefix = configuration["AWS:S3:RosterFilesPrefix"] ?? "rosters/";
    }

    /// <summary>
    /// Uploads file to S3 with organized key structure.
    /// Returns S3 key that can be used to retrieve the file later.
    /// </summary>
    /// <param name="fileStream">File content stream</param>
    /// <param name="fileName">Original filename</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>S3 key (e.g., "rosters/2026/02/07/guid/filename.xlsx")</returns>
    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        CancellationToken ct = default
    )
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required", nameof(fileName));

        // Create organized S3 key: rosters/yyyy/MM/dd/guid/filename
        var now = DateTime.UtcNow;
        var uniqueId = Guid.NewGuid();
        var key = $"{_rosterFilesPrefix}{now:yyyy/MM/dd}/{uniqueId}/{fileName}";

        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = GetContentType(fileName),
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256, // Enable server-side encryption
                Metadata =
                {
                    ["original-filename"] = fileName,
                    ["upload-date"] = now.ToString("O"),
                },
            };

            var response = await _s3Client.PutObjectAsync(putRequest, ct);

            _logger.LogInformation(
                "Successfully uploaded file to S3. Bucket: {Bucket}, Key: {Key}, ETag: {ETag}",
                _bucketName,
                key,
                response.ETag
            );

            return key;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload file to S3. Bucket: {Bucket}, Key: {Key}, Error: {ErrorCode}",
                _bucketName,
                key,
                ex.ErrorCode
            );
            throw new InvalidOperationException($"Failed to upload file to S3: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error uploading file to S3. Bucket: {Bucket}, Key: {Key}",
                _bucketName,
                key
            );
            throw;
        }
    }

    /// <summary>
    /// Retrieves file stream from S3.
    /// </summary>
    /// <param name="filePath">S3 key of the file</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>File stream or null if file not found</returns>
    public async Task<Stream?> GetFileStreamAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required", nameof(filePath));

        try
        {
            var getRequest = new GetObjectRequest { BucketName = _bucketName, Key = filePath };

            using var response = await _s3Client.GetObjectAsync(getRequest, ct);

            _logger.LogInformation(
                "Successfully retrieved file from S3. Bucket: {Bucket}, Key: {Key}, Size: {ContentLength}",
                _bucketName,
                filePath,
                response.ContentLength
            );

            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                "File not found in S3. Bucket: {Bucket}, Key: {Key}",
                _bucketName,
                filePath
            );
            return null;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to retrieve file from S3. Bucket: {Bucket}, Key: {Key}, Error: {ErrorCode}",
                _bucketName,
                filePath,
                ex.ErrorCode
            );
            throw new InvalidOperationException(
                $"Failed to retrieve file from S3: {ex.Message}",
                ex
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error retrieving file from S3. Bucket: {Bucket}, Key: {Key}",
                _bucketName,
                filePath
            );
            throw;
        }
    }

    /// <summary>
    /// Determines content type based on file extension.
    /// </summary>
    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream",
        };
    }

    /// <summary>
    /// Optional: Delete file from S3 (for retention policy or cleanup).
    /// Not in IFileStorageService interface but can be used internally.
    /// </summary>
    public async Task DeleteAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required", nameof(filePath));

        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = filePath,
            };

            await _s3Client.DeleteObjectAsync(deleteRequest, ct);

            _logger.LogInformation(
                "Successfully deleted file from S3. Bucket: {Bucket}, Key: {Key}",
                _bucketName,
                filePath
            );
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to delete file from S3. Bucket: {Bucket}, Key: {Key}, Error: {ErrorCode}",
                _bucketName,
                filePath,
                ex.ErrorCode
            );
            throw new InvalidOperationException($"Failed to delete file from S3: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error deleting file from S3. Bucket: {Bucket}, Key: {Key}",
                _bucketName,
                filePath
            );
            throw;
        }
    }
}
