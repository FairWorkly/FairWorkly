using System.Net.Http.Headers;
using System.Net.Http.Json;
using FairWorkly.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Infrastructure.AI.PythonServices;

public class PythonAiClient : IAiClient
{
    private readonly HttpClient _httpClient;

    public PythonAiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        // Default address for Python service
        var baseUrl = configuration["AiSettings:BaseUrl"] ?? "http://localhost:8000";
        _httpClient.BaseAddress = new Uri(baseUrl);
        // Keep FairBot chain timeout configurable and aligned with frontend/agent defaults.
        var timeoutSeconds = configuration.GetValue<int?>("AiSettings:TimeoutSeconds") ?? 120;
        if (timeoutSeconds <= 0)
        {
            timeoutSeconds = 120;
        }
        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Service-to-service authentication header for Agent Service.
        var serviceKey = configuration["AiSettings:ServiceKey"];
        if (string.IsNullOrWhiteSpace(serviceKey))
        {
            throw new InvalidOperationException(
                "AiSettings:ServiceKey is required. "
                    + "Set it in appsettings.json or via environment variable AiSettings__ServiceKey."
            );
        }
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Service-Key", serviceKey);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        // Convert the request object to a JSON string and send it
        var response = await _httpClient.PostAsJsonAsync(route, request, cancellationToken);

        // If the Python service reports an error, throw an exception
        response.EnsureSuccessStatusCode();

        // Read JSON from the response body and try to force it into the TResponse template
        // If the fields returned by Python don't match TResponse, there may be errors here or missing properties
        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken
        );

        if (result == null)
        {
            throw new InvalidOperationException("AI service returned null response.");
        }

        return result;
    }

    public async Task<TResponse> PostMultipartAsync<TResponse>(
        string route,
        Stream fileStream,
        string fileName,
        string contentType,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        using var content = new MultipartFormDataContent();

        // Add message parameter
        content.Add(new StringContent(message), "message");

        // Add file parameter
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(
            contentType ?? "application/octet-stream"
        );
        content.Add(streamContent, "file", fileName);

        // Send POST request
        var response = await _httpClient.PostAsync(route, content, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Read JSON response
        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken
        );

        if (result == null)
        {
            throw new InvalidOperationException("AI service returned null response.");
        }

        return result;
    }

    public async Task<TResponse> PostFormAsync<TResponse>(
        string route,
        Dictionary<string, string> formFields,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        using var content = new MultipartFormDataContent();

        foreach (var (key, value) in formFields)
        {
            content.Add(new StringContent(value), key);
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, route) { Content = content };
        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                request.Headers.TryAddWithoutValidation(key, value);
            }
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Agent Service returned {(int)response.StatusCode}: {errorBody}",
                null,
                response.StatusCode
            );
        }

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken
        );

        if (result == null)
        {
            throw new InvalidOperationException("AI service returned null response.");
        }

        return result;
    }
}
