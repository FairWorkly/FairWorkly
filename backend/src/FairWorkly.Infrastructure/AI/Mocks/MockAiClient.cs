using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Infrastructure.AI.Mocks;

public class MockAiClient : IAiClient
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        // Simulate Network Latency
        await Task.Delay(1000, cancellationToken);

        var resultData = MockAiRouter.Dispatch(route, request!);

        /*
         * The backend mock data is written as an anonymous object, but in real scenarios
         * the AI returns JSON, so here we need to first serialize the anonymous object
         * into JSON, then deserialize the JSON into the DTO.
         */
        var json = JsonSerializer.Serialize(resultData);
        var response = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);

        return response!;
    }

    public Task<TResponse> PostMultipartAsync<TResponse>(
        string route,
        Stream fileStream,
        string fileName,
        string contentType,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        // Mock implementation for roster file upload
        // In mock mode, roster upload is not supported
        throw new NotImplementedException(
            "Roster file upload is not supported in mock mode. "
                + "Set AiSettings:UseMockAi to false in appsettings.json to use the real Agent Service."
        );
    }
}
