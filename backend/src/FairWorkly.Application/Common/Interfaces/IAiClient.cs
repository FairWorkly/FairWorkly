namespace FairWorkly.Application.Common.Interfaces;

public interface IAiClient
{
    // Define a generic Post method for an HTTP POST call
    // TRequest: type of data sent to the AI
    // TResponse: type of data returned by the AI
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends multipart/form-data POST request with file upload to AI service.
    /// Used for uploading roster Excel files to Agent Service for parsing.
    /// </summary>
    /// <typeparam name="TResponse">Type of response from AI service</typeparam>
    /// <param name="route">API route (e.g., "/api/agent/chat")</param>
    /// <param name="fileStream">File stream to upload</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="contentType">Content type of the file (e.g., "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")</param>
    /// <param name="message">Message parameter for Agent Service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed response from AI service</returns>
    Task<TResponse> PostMultipartAsync<TResponse>(
        string route,
        Stream fileStream,
        string fileName,
        string contentType,
        string message,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends multipart/form-data POST request with text fields only (no file).
    /// Used for proxying FairBot chat requests to Agent Service.
    /// </summary>
    /// <typeparam name="TResponse">Type of response from AI service</typeparam>
    /// <param name="route">API route (e.g., "/api/agent/chat")</param>
    /// <param name="formFields">Key-value pairs to send as form fields</param>
    /// <param name="headers">Optional per-request headers (e.g., X-Request-Id)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed response from AI service</returns>
    Task<TResponse> PostFormAsync<TResponse>(
        string route,
        Dictionary<string, string> formFields,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    );
}
